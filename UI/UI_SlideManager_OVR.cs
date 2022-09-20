using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlideManager_OVR : MonoBehaviour // ui => ���� ���� UI, u => ����
{
    public GameObject[] u_Images; // ���� �̹������� ������ GameObject
    public MeshRenderer[] u_Render; // ���� ��Ʈ�ѷ��� ����� MeshRenderer

    [Header("UI Setting")]
    public float ui_DistanceX = 1; // 0��° �ε����� �������� x�� ���� ����
    public float ui_DistanceZ = 1; // 0��° �ε����� �������� z�� ���� ����
    public float ui_OpacityDistance = 1.5f; // x����� �ش� �Ÿ���ŭ �־����� ���� 0


    private bool isControllerInput = false; // ��Ʈ�ѷ��� �ԷµǾ����� ����
    private int u_Num;
    private float u_ImagesCurPos = 0; // u_Images�� 0��° �ε����� ���� x��
    public int u_ImagesIndex { get; private set; } = 0; // ���� ���� �ε�����, ���� �� �ش� ������ ����

    private void ControllerWait() // ��Ʈ�ѷ� �Է��� ���� �� �ְ���
    {
        isControllerInput = false;
    }

    private void UI_Opacity(GameObject u_Images) // �Ÿ��� ���� ���� ����
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
        for (int i = 0; i < u_Num; i++) // ĵ���� ���� �ʱ�ȭ
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

    public void UI_Slide(float LeftHand_JoystickInputX) // ���� ��Ʈ�ѷ��� ���̽�ƽ�� ���� �޾Ƽ� ���� ����â�� ui���� ������
    {
        if (!isControllerInput) // ��Ʈ�ѷ��Էµ� ���°� �ƴ϶�� ��Ʈ�ѷ��� �Է��ؼ� ���� ����
        {
            bool canMoveLeft = LeftHand_JoystickInputX < -0.5 && u_ImagesIndex > 0; // ���̽�ƽ ���� && u_ImagesIndex 0�ʰ�
            bool canMoveRight = LeftHand_JoystickInputX > 0.5 && u_ImagesIndex < u_Num - 1; // ���̽�ƽ ������ && u_ImagesIndex�� ������ �ε��� �̸�

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
        else // ����� �̹������� ��ġ�� ���������� �̿��� �̵�
        {
            for (int i = 0; i < u_Num; i++) // ui �̹������� ��ġ �̵�
            {
                float u_ImageX = i * ui_DistanceX;
                float u_ImageZ;
                if (u_Images[i].transform.localPosition.x < 0) // local�������� x���� ������ �Ǹ� �÷��̾�� ���� �־����� z�� ����
                {
                    u_ImageZ = -(ui_DistanceZ + (u_Images[i].transform.localPosition.x * ui_DistanceZ / ui_DistanceX));
                }
                else
                {
                    u_ImageZ = -(ui_DistanceZ - (u_Images[i].transform.localPosition.x * ui_DistanceZ / ui_DistanceX));
                }

                Vector3 targetPos = Vector3.right * (u_ImageX + u_ImagesCurPos) + Vector3.forward * u_ImageZ; // ��ǥ ��ġ�� x���� z�� �Է�

                if (Vector3.Distance(u_Images[i].transform.localPosition, targetPos) > 0.03f) // �Ÿ� 0.03�ʰ� targetPos�� ��������
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