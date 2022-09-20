using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FadeEffect : MonoBehaviour // f => Fade // p => �÷��̾�
{
    [Header("PlayerCtrl")]
    public PlayerCtrl p_Ctrl = null; // ���Ӿ� �Ǵ� ����

    [Header("FadeEffect")]
    public static Image f_Img;
    public static float f_ImgA;
    public static bool isFading; // ���̵� ����
    private bool isFadeOut = false; // ���Ӿ� ���̵� �����
    public static bool reloadScene; // ���̵� �� �� �̵� ����

    private void Start()
    {
        ResetState();
        f_Img = GetComponent<Image>();
        StartCoroutine(OpenFade());

    }
    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        reloadScene = false;
        isFading = true;
        f_Img = null;
        f_ImgA = 1;

    }
    private void Update()
    {
        f_Img.color = new Color(0, 0, 0, f_ImgA);

        if (p_Ctrl != null) // ���Ӿ� �� ���
        {
            if (p_Ctrl.isSet == false && PlayerCtrl.p_State == PlayerState.normal && !isFadeOut) // �÷��̾� �߻� �� ��ġ & ���̵忩�� Ȯ��
            {
                isFadeOut = true;
                StartCoroutine(moveFadeOut());
            }
        }
    }


    private IEnumerator moveFadeOut() // ���Ӿ� ���� �÷��̾� ��ġ�̵� + ���̵�ȿ��
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
    private IEnumerator moveFadeIn() // ���Ӿ� ���� �÷��̾� ��ġ�̵� + ���̵�ȿ�� 2
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

    public static IEnumerator OpenFade() // �� ����� Fadeȿ��
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

    public static IEnumerator SceneFade(string sceneName) // �� ���Խ� Fadeȿ��
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

    //public static IEnumerator LoadSceneAsync(string sceneName) // PC�� ��ũ�� ���������� APK��ȯ �� ���� �� ������� ����
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
