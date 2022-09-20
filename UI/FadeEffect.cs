using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FadeEffect : MonoBehaviour // f => Fade // p => 플레이어
{
    [Header("PlayerCtrl")]
    public PlayerCtrl p_Ctrl = null; // 게임씬 판단 여부

    [Header("FadeEffect")]
    public static Image f_Img;
    public static float f_ImgA;
    public static bool isFading; // 페이드 여부
    private bool isFadeOut = false; // 게임씬 페이드 제어용
    public static bool reloadScene; // 페이드 후 씬 이동 여부

    private void Start()
    {
        ResetState();
        f_Img = GetComponent<Image>();
        StartCoroutine(OpenFade());

    }
    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        reloadScene = false;
        isFading = true;
        f_Img = null;
        f_ImgA = 1;

    }
    private void Update()
    {
        f_Img.color = new Color(0, 0, 0, f_ImgA);

        if (p_Ctrl != null) // 게임씬 일 경우
        {
            if (p_Ctrl.isSet == false && PlayerCtrl.p_State == PlayerState.normal && !isFadeOut) // 플레이어 발사 후 위치 & 페이드여부 확인
            {
                isFadeOut = true;
                StartCoroutine(moveFadeOut());
            }
        }
    }


    private IEnumerator moveFadeOut() // 게임씬 전용 플레이어 위치이동 + 페이드효과
    {
        isFading = true;

        while (true)
        {
            f_ImgA += Time.deltaTime;
            if(f_ImgA >= 1)
            {
                p_Ctrl.transform.position = p_Ctrl.p_PosVec;
                f_ImgA = 1;
                GetComponent<Image>().color = new Color(0, 0, 0, f_ImgA);
                isFading = false;

                isFadeOut = false;
                StartCoroutine(moveFadeIn());
                break;
            }
            yield return null;
        }
    }
    private IEnumerator moveFadeIn() // 게임씬 전용 플레이어 위치이동 + 페이드효과 2
    {
        isFading = true;

        while (true)
        {
            f_ImgA -= Time.deltaTime;
            if (f_ImgA <= 0)
            {
                f_ImgA = 0;
                GetComponent<Image>().color = new Color(0, 0, 0, f_ImgA);
                isFading = false;

                break;
            }
            yield return null;
        }
    }

    public static IEnumerator OpenFade() // 씬 입장시 Fade효과
    {
        isFading = true;

        while (true)
        {
            
            f_ImgA -= Time.deltaTime *0.8f;
            if(f_ImgA <= 0)
            {
                f_Img.color = new Color(0, 0, 0, f_ImgA);
                isFading = false;

                break;
            }
            yield return null;
        }
    }

    public static IEnumerator SceneFade(string sceneName) // 씬 진입시 Fade효과
    {
        isFading = true;
        reloadScene = true;

        while (true)
        {
            f_ImgA += Time.deltaTime *0.8f;
            if (f_ImgA >= 1)
            {
                f_ImgA = 1;
                f_Img.color = new Color(0, 0, 0, f_ImgA);
                isFading = false;
                

                SceneManager.LoadScene(sceneName);

                break;
            }
            yield return null;
        }

    }

    //public static IEnumerator LoadSceneAsync(string sceneName) // PC와 링크시 구동되지만 APK변환 후 실행 중 미적용돼 보류
    //{
    //    AsyncOperation AsyncLoad = SceneManager.LoadSceneAsync(sceneName);
    //    AsyncLoad.allowSceneActivation = false;
    //    while (!AsyncLoad.isDone)
    //    {
    //        yield return null;
    //        if (AsyncLoad.progress >= 0.9f)
    //        {
    //            loadingDone = true;
    //        }
    //    }
    //}
    
    

}
