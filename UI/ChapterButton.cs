using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterButton : MonoBehaviour
{
    private bool isClicked; // 클릭여부 확인

    [Header("Enable Objs")]
    public GameObject[] enable_Objs; // 클릭시 활성화 될 것들

    [Header("Disable Objs")]
    public GameObject[] disable_Objs; // 클릭시 비활성화 될 것들



    void Start()
    {
        ResetState();
        
        disable_Objs[0].SetActive(true);
        for(int i = 0; i < enable_Objs.Length; i++)
        {
            enable_Objs[i].transform.localPosition = Vector3.zero;
            enable_Objs[i].SetActive(false);
        }
    }
    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        isClicked = false;

    }


    void Update()
    {
        if (isClicked)
        {
            for (int i = 0; i < disable_Objs.Length; i++)
            {
                disable_Objs[i].SetActive(false);
            }
            for (int i = 0; i < enable_Objs.Length; i++)
            {
                enable_Objs[i].SetActive(true);
                enable_Objs[i].transform.localPosition = Vector3.Lerp(enable_Objs[i].transform.localPosition,
                    Vector3.zero + new Vector3(-165 + (i * 165), 0, -10), 3 * Time.deltaTime);
            }
        }
        else
        {
            for (int i = 0; i < enable_Objs.Length; i++)
            {
                enable_Objs[i].transform.localPosition = Vector3.zero;
                enable_Objs[i].SetActive(false);
            }
            for (int i = 0; i < disable_Objs.Length; i++)
            {
                disable_Objs[i].SetActive(true);
            }
        }

    }
    
    public void ClickCptBtn()
    {
        isClicked = true;
    }
    public void EscapeBtn()
    {
        isClicked=false;

    }

}
