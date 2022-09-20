using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BtnType // ��ư ����
{
    Level, Exit, Home, Restart, nextLevel, Sound, Vibration
}
public class UiBtn : MonoBehaviour
{
    [Header("Button Type")]
    public BtnType btnType;

    [Header("Type Level")]
    public GameObject[] level_Objs = null; // ���� ����
    public GameObject level_LockImg = null; // ���� ��� �̹��� 
    public bool isLocked = true; // ��ݿ���
    private int score; // �ش� ���� ����
    public int level; // �ε� �� ����



    [Header("Type Sound & Vibration")]
    public GameObject[] sv_imgs = null;
    public AudioClip[] audioClips = null;
    private AudioSource audioSource;


    void Awake()
    {
        
        if (GetComponent<AudioSource>() != null) // ��� ��ư�� ������� ���� �ʾ� ����ó��
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        
        // ���ھ� üũ �� �ݿ�
        score = PlayerPrefs.GetInt((level - 1).ToString(), 0);
        if (level > 1 && score < 1)
        {
            isLocked = true;
            score = 0;
        }
        else
        {
            isLocked = false;
            score = PlayerPrefs.GetInt(level.ToString(), 0);
        }



        if (btnType == BtnType.Level) //���� ��ư�Ͻ� ��ư Ȱ��ȭ ���� + ���� ����
        {
            GetComponent<Collider>().enabled = !isLocked;
            level_LockImg.SetActive(isLocked);

            if (isLocked)
            {
                for (int i = 0; i < level_Objs.Length; i++)
                {
                    level_Objs[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i <score+1; i++)
                {
                    level_Objs[i].SetActive(true); // ����
                }
            }
        }


    }

    void Start()
    {
        // ���� ���� �̹��� ����
        if (btnType == BtnType.Sound)
        {
            sv_imgs[0].SetActive(!SoundVibOnOff.soundOn);
            sv_imgs[1].SetActive(SoundVibOnOff.soundOn);
        }
        else if (btnType == BtnType.Vibration)
        {
            sv_imgs[0].SetActive(!SoundVibOnOff.vibOn);
            sv_imgs[1].SetActive(SoundVibOnOff.vibOn);
        }
    }



    public void ClickBtn() // �÷��̾ Ray�� ����� ��ư Ŭ���� �ߵ� , �÷����� PC�� �ƴ� Oculus Quest(VR) ��� �̱� ������ Ray����� �����ϴ� ������ �������� �����ص�
    {
        // ��ư Ÿ�Ժ� ����
        if (btnType == BtnType.Level) // ������ Level�� �̵�
        {
            if (isLocked)
            {
                audioSource.clip = audioClips[1];
                audioSource.Play();
            }
            else
            {
                audioSource.clip = audioClips[0];
                audioSource.Play();
                Invoke("SoundChange", 0.7f);

                StartCoroutine(FadeEffect.SceneFade("Scene" + level.ToString()));
                //StartCoroutine(FadeEffect.LoadSceneAsync("Scene" + level.ToString()));
            }
        }
        else if (btnType == BtnType.Home) // StartScene���� �̵�
        {
            StartCoroutine(FadeEffect.SceneFade("StartScene"));

        }
        else if (btnType == BtnType.Restart) // ����� �ٽý���
        {
            StartCoroutine(FadeEffect.SceneFade(SceneManager.GetActiveScene().name));

        }
        else if (btnType == BtnType.Exit) // ���� ����
        {
            Application.Quit();
        }
        else if ((btnType == BtnType.nextLevel)) // ������ ���� ������ �̵� (Chapter ���� ������ ������ Ȯ���� �־� ����� ������ ��������)
        {
            StartCoroutine(FadeEffect.SceneFade("Scene" + level.ToString()));
        }
        else if (btnType == BtnType.Sound) // ���� OnOff
        {
            sv_imgs[1].SetActive(!SoundVibOnOff.soundOn);
            sv_imgs[0].SetActive(SoundVibOnOff.soundOn);

            SoundVibOnOff.TurnSound();
            audioSource.clip = audioClips[0];
            audioSource.Play();
            PlayerCtrl.p_State = PlayerState.normal;

        }
        else if (btnType == BtnType.Vibration) // ���� OnOff
        {
            sv_imgs[0].SetActive(SoundVibOnOff.vibOn);
            sv_imgs[1].SetActive(!SoundVibOnOff.vibOn);

            if (SoundVibOnOff.vibOn == false)
            {
                StartCoroutine(VibrationAfterSkillUses(0.4f));
            }
            SoundVibOnOff.TurnVib();

            PlayerCtrl.p_State = PlayerState.normal;
        }

    }
    private void SoundChange() // ���� ü���� 
    {
        audioSource.clip = audioClips[2];
        audioSource.Play();
    }

    public static IEnumerator VibrationAfterSkillUses(float waitTime = 0.1f) // ��Ʈ�ѷ� ���� ���� 
    {
        OVRInput.SetControllerVibration(0.2f, 0.2f, OVRInput.Controller.Touch);
        yield return new WaitForSeconds(waitTime);
        OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.Touch);
    }
}
