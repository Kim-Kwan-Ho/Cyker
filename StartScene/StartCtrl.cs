using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartCtrl : MonoBehaviour  // p => 플레이어 , pRH => 플레이어 우측 스위치 , l => 라인렌더러(레이저)
{
    [Header("Player Target Position")]
    public Vector3 p_TargetVec; // 플레이어 목표 위치


    [Header("Before Enter Objs")]
    public GameObject[] dis_Objs; // 진입 후 비활성화 오브젝트
    public Text start_Txt; // 안내문구
    private float text_A;
    private bool text_Up;


    [Header("After Enter Objs")]
    public GameObject[] ena_Objs; // 진입 후 활성화 오브젝트
    public Image[] ena_ImgsAlpha;
    public Text[] ena_TxtAlpha;


    [Header("Check Click & Entered")]
    private bool isClicked; // 클릭 여부
    private bool isEntered; // 진입 여부
    private float img_A;


    [Header("Laser")]
    private LineRenderer pRH_Laser;
    private bool l_Check; // 레이저 활성화 제어



    void Start()
    {
        ResetState(); 

        StartCoroutine("TextAlphaChange");
        
        pRH_Laser = GetComponentInChildren<LineRenderer>();
    }

    void Update()
    {
        start_Txt.color = new Color(start_Txt.color.r, start_Txt.color.g, start_Txt.color.b, text_A);


        if (isClicked==false && OVRInput.GetDown(OVRInput.Button.Any) && !OVRInput.GetDown(OVRInput.Button.Right)) // 컨트를러 클릭 확인
        {
            isClicked=true;
        }


        if (isClicked) // 클릭시 플레이어 목표지점으로 이동 및 각종 Fade효과 적용
        {
            transform.position = Vector3.Lerp(transform.position, p_TargetVec, 1 * Time.deltaTime);

            for(int i=0; i<ena_ImgsAlpha.Length; i++)
            {
                ena_ImgsAlpha[i].color = new Color(1,1,1, img_A);
            }
            for(int i=0; i<ena_TxtAlpha.Length; i++)
            {
                ena_TxtAlpha[i].color = new Color(1,1,1, img_A);
            }
            if (isEntered == false)
            {
                StartCoroutine("EnterScene");
            }

        }
        if (isEntered && !pRH_Laser.enabled && l_Check == false) // 우측 컨트롤러 레이저 활성화
        {
            pRH_Laser.enabled = true;
            l_Check = true;
        }

    }
    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        l_Check = false;
        text_Up = false;
        isEntered = false;
        isClicked = false;
        text_A = 1.0f;
        img_A = 0;
        for (int i = 0; i < dis_Objs.Length; i++)
        {
            dis_Objs[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < ena_Objs.Length; i++)
        {
            ena_Objs[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator EnterScene() // 씬 진입
    {
        yield return new WaitForSeconds(1.3f);
        isEntered = true;
        for (int i=0; i < dis_Objs.Length; i++)
        {
            dis_Objs[i].gameObject.SetActive(false);
        }
        for(int i=0; i<ena_Objs.Length; i++)
        {
            ena_Objs[i].gameObject.SetActive(true);
        }
        StartCoroutine("ImageAlphaChange");
    }

    private IEnumerator ImageAlphaChange() // 진입시 Fade효과
    {
        while (true)
        {
            if (img_A <= 1)
            {
                img_A += Time.deltaTime * 0.5f;
            }
            else
            {
                img_A = 1;
                break;
            }
            yield return null;
        }
    }
    private IEnumerator TextAlphaChange() // Text Fade효과
    {
        while (true)
        {
            if (text_Up)
            {
                text_A += Time.deltaTime * 0.7f;
                if (text_A >= 1)
                {
                    text_Up=false;
                }
            }
            else
            {
                text_A -= Time.deltaTime * 0.7f;
                if (text_A <= 0)
                {
                    text_Up = true;
                }
            }
            yield return null;
        }
    }
}
