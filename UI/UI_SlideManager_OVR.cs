using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlideManager_OVR : MonoBehaviour // ui => 유닛 구매 UI, u => 유닛
{
    public GameObject[] u_Images; // 유닛 이미지들을 포함한 GameObject
    public MeshRenderer[] u_Render; // 왼쪽 컨트롤러에 띄워줄 MeshRenderer

    [Header("UI Setting")]
    public float ui_DistanceX = 1; // 0번째 인덱스를 기준으로 x값 증가 정도
    public float ui_DistanceZ = 1; // 0번째 인덱스를 기준으로 z값 증가 정도
    public float ui_OpacityDistance = 1.5f; // x축기준 해당 거리만큼 멀어지면 투명도 0


    private bool isControllerInput = false; // 컨트롤러가 입력되었는지 여부
    private int u_Num;
    private float u_ImagesCurPos = 0; // u_Images의 0번째 인덱스의 현재 x값
    public int u_ImagesIndex { get; private set; } = 0; // 현재 유닛 인덱스값, 선택 시 해당 유닛이 선택

    private void ControllerWait() // 컨트롤러 입력을 받을 수 있게함
    {
        isControllerInput = false;
    }

    private void UI_Opacity(GameObject u_Images) // 거리에 따른 투명도 설정
    {
        Image[] child_images = u_Images.GetComponentsInChildren<Image>();
        Text[] child_texts = u_Images.GetComponentsInChildren<Text>();
        foreach (Text text in child_texts)
        {
            Color colorA = text.color;
            colorA.a = ((ui_OpacityDistance - Mathf.Abs(u_Images.transform.localPosition.x)) / ui_OpacityDistance);
            text.color = colorA;
        }
        foreach (Image image in child_images)
        {
            Color colorA = image.color;
            colorA.a = ((ui_OpacityDistance - Mathf.Abs(u_Images.transform.localPosition.x)) / ui_OpacityDistance);
            image.color = colorA;
        }
    }

    void Start()
    {
        u_Num = u_Images.Length;
        u_Render[u_ImagesIndex].enabled = true;
        for (int i = 0; i < u_Num; i++) // 캔버스 설정 초기화
        {
            float u_ImageX = i * ui_DistanceX;
            float u_ImageZ = i * ui_DistanceZ - ui_DistanceZ;

            u_Images[i].transform.localPosition = Vector3.right * u_ImageX + Vector3.forward * u_ImageZ;

            UI_Opacity(u_Images[i]);
        }
    }


    public void U_RenderEnable(bool isEnable) //left controller Unit Render On/Off
    {
        u_Render[u_ImagesIndex].enabled = isEnable;
    }

    public void UI_Slide(float LeftHand_JoystickInputX) // 왼쪽 컨트롤러의 조이스틱의 값을 받아서 유닛 선택창의 ui들을 움직임
    {
        if (!isControllerInput) // 컨트롤러입력된 상태가 아니라면 컨트롤러를 입력해서 유닛 변경
        {
            bool canMoveLeft = LeftHand_JoystickInputX < -0.5 && u_ImagesIndex > 0; // 조이스틱 왼쪽 && u_ImagesIndex 0초과
            bool canMoveRight = LeftHand_JoystickInputX > 0.5 && u_ImagesIndex < u_Num - 1; // 조이스틱 오른쪽 && u_ImagesIndex가 마지막 인덱스 미만

            if (canMoveLeft || canMoveRight)
            {
                GetComponent<AudioSource>().Play();
                isControllerInput = true;
                Invoke("ControllerWait", 0.3f);
                u_Render[u_ImagesIndex].enabled = false;
                if (canMoveLeft)
                {
                    u_ImagesCurPos += ui_DistanceX;
                    u_ImagesIndex--;
                }
                else if (canMoveRight)
                {
                    u_ImagesCurPos -= ui_DistanceX;
                    u_ImagesIndex++;
                }
                u_Render[u_ImagesIndex].enabled = true;
            }
        }
        else // 변경된 이미지들의 위치를 선형보간을 이용해 이동
        {
            for (int i = 0; i < u_Num; i++) // ui 이미지들의 위치 이동
            {
                float u_ImageX = i * ui_DistanceX;
                float u_ImageZ;
                if (u_Images[i].transform.localPosition.x < 0) // local포지션의 x값이 음수가 되면 플레이어로 부터 멀어지게 z값 조정
                {
                    u_ImageZ = -(ui_DistanceZ + (u_Images[i].transform.localPosition.x * ui_DistanceZ / ui_DistanceX));
                }
                else
                {
                    u_ImageZ = -(ui_DistanceZ - (u_Images[i].transform.localPosition.x * ui_DistanceZ / ui_DistanceX));
                }

                Vector3 targetPos = Vector3.right * (u_ImageX + u_ImagesCurPos) + Vector3.forward * u_ImageZ; // 목표 위치의 x값과 z값 입력

                if (Vector3.Distance(u_Images[i].transform.localPosition, targetPos) > 0.03f) // 거리 0.03초과 targetPos로 선형보간
                {
                    u_Images[i].transform.localPosition = Vector3.Lerp(u_Images[i].transform.localPosition, targetPos, 0.2f);
                }
                else
                {
                    u_Images[i].transform.localPosition = targetPos;
                }

                UI_Opacity(u_Images[i]);
            }
        }
    }
}