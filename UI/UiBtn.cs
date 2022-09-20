using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BtnType // 버튼 종류
{
    Level, Exit, Home, Restart, nextLevel, Sound, Vibration
}
public class UiBtn : MonoBehaviour
{
    [Header("Button Type")]
    public BtnType btnType;

    [Header("Type Level")]
    public GameObject[] level_Objs = null; // 레벨 정보
    public GameObject level_LockImg = null; // 레벨 잠금 이미지 
    public bool isLocked = true; // 잠금여부
    private int score; // 해당 레벨 점수
    public int level; // 로드 할 레벨



    [Header("Type Sound & Vibration")]
    public GameObject[] sv_imgs = null;
    public AudioClip[] audioClips = null;
    private AudioSource audioSource;


    void Awake()
    {
        
        if (GetComponent<AudioSource>() != null) // 모든 버튼에 오디오가 있지 않아 예외처리
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        
        // 스코어 체크 및 반영
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



        if (btnType == BtnType.Level) //레벨 버튼일시 버튼 활성화 관리 + 별점 관리
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
                    level_Objs[i].SetActive(true); // 레벨
                }
            }
        }


    }

    void Start()
    {
        // 사운드 진동 이미지 설정
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



    public void ClickBtn() // 플레이어가 Ray를 사용해 버튼 클릭시 발동 , 플랫폼이 PC가 아닌 Oculus Quest(VR) 기기 이기 떄문에 Ray방식이 적합하다 생각해 공용으로 설정해둠
    {
        // 버튼 타입별 나열
        if (btnType == BtnType.Level) // 설정한 Level로 이동
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
        else if (btnType == BtnType.Home) // StartScene으로 이동
        {
            StartCoroutine(FadeEffect.SceneFade("StartScene"));

        }
        else if (btnType == BtnType.Restart) // 현재씬 다시시작
        {
            StartCoroutine(FadeEffect.SceneFade(SceneManager.GetActiveScene().name));

        }
        else if (btnType == BtnType.Exit) // 게임 종료
        {
            Application.Quit();
        }
        else if ((btnType == BtnType.nextLevel)) // 설정한 다음 레벨로 이동 (Chapter 변경 때문에 변동될 확률이 있어 현재는 레벨을 지정해줌)
        {
            StartCoroutine(FadeEffect.SceneFade("Scene" + level.ToString()));
        }
        else if (btnType == BtnType.Sound) // 사운드 OnOff
        {
            sv_imgs[1].SetActive(!SoundVibOnOff.soundOn);
            sv_imgs[0].SetActive(SoundVibOnOff.soundOn);

            SoundVibOnOff.TurnSound();
            audioSource.clip = audioClips[0];
            audioSource.Play();
            PlayerCtrl.p_State = PlayerState.normal;

        }
        else if (btnType == BtnType.Vibration) // 진동 OnOff
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
    private void SoundChange() // 사운드 체인지 
    {
        audioSource.clip = audioClips[2];
        audioSource.Play();
    }

    public static IEnumerator VibrationAfterSkillUses(float waitTime = 0.1f) // 컨트롤러 진동 설정 
    {
        OVRInput.SetControllerVibration(0.2f, 0.2f, OVRInput.Controller.Touch);
        yield return new WaitForSeconds(waitTime);
        OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.Touch);
    }
}
