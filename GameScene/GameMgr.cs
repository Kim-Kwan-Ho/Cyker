using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState // 게임 진행사항
{
    playing, pause, clear, fail //게임중, 퍼즈, 클리어, 실패
}

public class GameMgr : MonoBehaviour // g => 게임 시스템, p => 플레이어, e => 적, u => 유닛, s => SkyBox
{
    [Header("Game Manager")]
    public GameState g_State; // 현재 게임 진행상황
    public int g_Level; // 현재 레벨
    public int g_ScoreBuyCount; // 점수요건 구매 횟수 (설정한 구매횟수 이하로 구매 후 클리어시 점수추가)
    public int g_ScoreEnergyCount; // 점수요건 잔여 에너지 (설정한 에너지 이상 남기고 클리어시 점수추가)
    private AudioSource g_AudioSource; // 게임 매니저 오디오
    public AudioClip[] g_AudioClips; // 0 => 유닛 구매 불가 사운드 1=> 유닛 구매 사운드

    [Header("Unit Setting")] // 현재 게임 미션, 트리거에 관해 상의중이기 때문에 해당 스테이지에 사용 가능한 유닛, 가격, 텍스트를 가장 간단한 방법으로 설정
    public GameObject[] u_Setting; // 해당 판에 사용 가능한 유닛
    public int[] u_Cost; // 유닛 가격 설정
    public GameObject[] u_Cost_Txt; // 유닛 가격 표시란

    [Header("Game Player")]
    public PlayerCtrl playerCtrl;
    public int p_MaxEnergy; // 최대 에너지 (판마다 할당된 에너지가 다름)
    public int p_CurEnergy { get; private set; } // 현재 에너지
    private int p_Score; // 현재 스코어(표출용)
    private int p_BestScore; // 최고 스코어(기록용)
    private int p_BuyCount; // 플레이어 구매 횟수
    private GameObject p_CurUnit; // 플레이어가 구매한 유닛 (데이터 관리용)


    [Header("Enemy")]
    public GameObject e_CountTxt; // 잔여 적 숫자 표시 텍스트(자식들 텍스트 조정을 위해 상위 부모를 받아옴)
    [HideInInspector]
    public int e_Count; // 적 숫자

    [Header("UI")]
    public GameObject ui_InfoSetting; // 환경설정창 및 정보창 (환경설정을 키면 정보도 같이 나와 한 번에 설정)
    public Text[] ui_ResultTxts; // 결과 텍스트 표시
    public GameObject ui_EnergyPnl; // 잔여 에너지를 표시해주는 판넬
    public Image[] ui_ScoreBImgs; // 클리어시 점수를 표시해주는 이미지 (Back)
    public Image[] ui_ScoreFImgs; // 클리어시 점수를 표시해주는 이미지 (Front) 잔상 효과로 인해 Back, Front 구분함
    public GameObject[] ui_ResultBtn = new GameObject[2]; // 재시작 0 , 다음레벨 1 (게임 실패시 or 마지막 챕터면 재시작 / 그게 아닐경우 게임 성공하면 다음레벨) 버튼 설정

    [Header("SkyBox")]
    public Color32 s_StartC; // SkyBox TintColor는 저장된 값이기 때문에 항상 초기값 설정이 필요해 선언함 (클리어시 SkyBox TintColor 증가하게 설정해둠)



    void Awake()
    {
        //해당 스테이지의 점수를 불러옴, 저장된 점수가 없을 경우 0으로 불러옴
        p_BestScore = PlayerPrefs.GetInt(g_Level.ToString(), 0);
        e_Count = GameObject.FindGameObjectsWithTag("Enemy").Length; // 게임 내에 존재하는 적 갯수 찾기

    }
    void Start()
    {
        ResetState();
        g_AudioSource = GetComponent<AudioSource>();
    }

    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        p_BuyCount = 0;
        p_Score = 0;
        g_State = GameState.playing;
        p_CurEnergy = p_MaxEnergy;

        RenderSettings.skybox.SetColor("_Tint", s_StartC); // SkyBox TintColor 초기값 지정(어두운색)


        Text[] e_CountTxts = this.e_CountTxt.GetComponentsInChildren<Text>();
        foreach (Text text in e_CountTxts) // 잔여 적 텍스트로 표시
        {
            text.text = e_Count.ToString();
        }


        for (int i = 0; i < ui_EnergyPnl.transform.childCount; i++)  // 에너지 판넬 에너지 모두 비활성화
        {
            ui_EnergyPnl.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < p_CurEnergy; i++) // 에너지 판넬 에너지 만큼 활성화
        {
            ui_EnergyPnl.transform.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < u_Cost_Txt.Length; i++) // 유닛 가격 텍스트 지정
        {
            Text[] texts = u_Cost_Txt[i].GetComponentsInChildren<Text>();
            for (int k = 0; k < texts.Length; k++)
            {
                texts[k].text = u_Cost[i].ToString();
            }
        }
    }

    public void BuyUnit(int UnitType) // 인자 전달받아서 해당 배열에 있는 유닛 구매
    {
        if (p_CurEnergy < u_Cost[UnitType]) // 현재 에너지가 유닛 가격보다 적을경우 
        {
            g_AudioSource.clip = g_AudioClips[0];
            g_AudioSource.Play();
            return;
        }
        else // 현재 에너지가 유닛 가격과 같거나 많을 경우 (구매가능)
        {
            p_CurEnergy -= u_Cost[UnitType]; // 에너지 감소
            g_AudioSource.clip = g_AudioClips[1];
            g_AudioSource.Play();
            p_BuyCount++; // 구매횟수 증가
            for (int i = p_MaxEnergy-1; i >= p_CurEnergy; i--) // 에너지 판넬 표시갯수 차감
            {
                ui_EnergyPnl.transform.GetChild(i).gameObject.SetActive(false);
            }

            p_CurUnit = Instantiate(u_Setting[UnitType], playerCtrl.transform.position, Quaternion.identity) as GameObject; // 유닛 구매 후 p_CurUnit에 구매한 유닛 저장
            PlayerLSwitch.pLH_SelectedU = p_CurUnit.GetComponent<UnitMain>(); // pLH에 구매한 유닛 전달
            p_CurUnit.GetComponent<UnitMain>().u_State = UnitState.fireready; // 유닛에 발사 전 상태로 State변경
            PlayerCtrl.p_State = PlayerState.aim; // 플레이어 조준 상태로 변경
        }
        
    }

    public void DecreaseEnemy() // 적 갯수 차감 (적 파괴시 발동)
    {
        e_Count--; // 적 갯수 감소

        Text[] e_CountTxts = this.e_CountTxt.GetComponentsInChildren<Text>(); // 잔여적 텍스트 표시갯수 차감
        foreach (Text text in e_CountTxts)
        {
            text.text = e_Count.ToString();
        }

        if (e_Count <= 0) // 잔여적이 없을 경우
        {
            g_State = GameState.clear; // 게임 클리어

            // 플레이어 점수 배점 (현재는 클리어시 1점 유닛 구매횟수 1점 에너지 갯수 1점으로 총 3점 배정)
            p_Score += 1;
            if (p_BuyCount <= g_ScoreBuyCount)
            {
                p_Score += 1;
            }
            if(p_CurEnergy >= g_ScoreEnergyCount)
            {
                p_Score += 1;
            }


            for (int i = ui_ScoreFImgs.Length; i > p_Score; i--) // 점수에 맞춰 이미지 색조정
            {
                ui_ScoreFImgs[i-1].color = new Color(0.5f, 0.5f, 0.5f);
            }

            // 해당 점수가 최대 점수보다 크면 플레이어 점수 저장 (기존 최대 점수가 없을시 0으로 자동배정됨) 
            if (p_Score > p_BestScore)
            {
                PlayerPrefs.SetInt(g_Level.ToString(), p_Score);
            }
            else
            {
                return;
            }

        }
    }
    public void DecreaseUnit() // 유닛이 사라지면서 호출 => 에너지 모두 사용시 클리어체크
    {
        if(p_CurEnergy<=0)
        {
            StartCoroutine("ClearCheck");
        }
        else
        {
            return;
        }
    }

    private IEnumerator ClearCheck() //게임 클리어여부 확인
    {

        yield return new WaitForSeconds(5f); //적이 외부 충격으로 죽을 확률을 생각해 5초뒤 실행

        if (FadeEffect.reloadScene) // 에러방지 (Fade효과가 적용중일때는 다른씬 이동중이기 때문에 사전차단)
            yield break;

        if (e_Count >= 1) // 적 잔여시 패배 아닐시 클리어
        {
            g_State = GameState.fail;
        }
        else // 잔여적이 없을시 
        {
            p_Score = 1; // 에너지를 다 소모하면 구매횟수 또한 성립하지 못하기 때문에 1점 배정

            
            if (p_Score > p_BestScore) // 플레이어 점수 저장
            {
                PlayerPrefs.SetInt(g_Level.ToString(), p_Score);
            }

            g_State = GameState.clear; // 클리어 설정
        }
    }



    void Update()   
    {
        if (g_State == GameState.clear) // 클리어 시
        {
            if(playerCtrl.isSet) // 플레이어가 재 위치로 왔을때 SkyBox 밝아짐 설정
            {
                RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), Color.white, Time.deltaTime * 0.6f));
            }
        }

        switch (g_State) // 게임 상태에 맞춰 UI 설정
        {
            case GameState.playing:
                ui_InfoSetting.SetActive(false);
                break;

            case GameState.pause: // TimeScale을 0으로 하기보다 게임에 pause라는 State를 만들어 타 행동을 못하게 설정
                ui_InfoSetting.SetActive(true);
                break;

            case GameState.clear:
                ui_InfoSetting.SetActive(false);
                if (g_Level != 3)
                {
                    ui_ResultBtn[0].SetActive(false);
                    ui_ResultBtn[1].SetActive(true);
                }
                else
                {
                    ui_ResultBtn[0].SetActive(true);
                    ui_ResultBtn[1].SetActive(false);
                }
                
                ui_ResultTxts[0].text = "GAME CLEAR";
                ui_ResultTxts[1].text = "GAME CLEAR";
                break;

            case GameState.fail:
                for (int i = 0; i < 3; i++)
                {
                    ui_ScoreFImgs[i].enabled = false;
                    ui_ScoreBImgs[i].enabled = false;
                }
                ui_InfoSetting.SetActive(false);
                ui_ResultBtn[0].SetActive(true);
                ui_ResultBtn[1].SetActive(false);
                ui_ResultTxts[0].text = "GAME OVER";
                ui_ResultTxts[1].text = "GAME OVER";
                break;
        }
    }

}
