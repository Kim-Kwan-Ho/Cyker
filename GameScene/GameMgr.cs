using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState // ���� �������
{
    playing, pause, clear, fail //������, ����, Ŭ����, ����
}

public class GameMgr : MonoBehaviour // g => ���� �ý���, p => �÷��̾�, e => ��, u => ����, s => SkyBox
{
    [Header("Game Manager")]
    public GameState g_State; // ���� ���� �����Ȳ
    public int g_Level; // ���� ����
    public int g_ScoreBuyCount; // ������� ���� Ƚ�� (������ ����Ƚ�� ���Ϸ� ���� �� Ŭ����� �����߰�)
    public int g_ScoreEnergyCount; // ������� �ܿ� ������ (������ ������ �̻� ����� Ŭ����� �����߰�)
    private AudioSource g_AudioSource; // ���� �Ŵ��� �����
    public AudioClip[] g_AudioClips; // 0 => ���� ���� �Ұ� ���� 1=> ���� ���� ����

    [Header("Unit Setting")] // ���� ���� �̼�, Ʈ���ſ� ���� �������̱� ������ �ش� ���������� ��� ������ ����, ����, �ؽ�Ʈ�� ���� ������ ������� ����
    public GameObject[] u_Setting; // �ش� �ǿ� ��� ������ ����
    public int[] u_Cost; // ���� ���� ����
    public GameObject[] u_Cost_Txt; // ���� ���� ǥ�ö�

    [Header("Game Player")]
    public PlayerCtrl playerCtrl;
    public int p_MaxEnergy; // �ִ� ������ (�Ǹ��� �Ҵ�� �������� �ٸ�)
    public int p_CurEnergy { get; private set; } // ���� ������
    private int p_Score; // ���� ���ھ�(ǥ���)
    private int p_BestScore; // �ְ� ���ھ�(��Ͽ�)
    private int p_BuyCount; // �÷��̾� ���� Ƚ��
    private GameObject p_CurUnit; // �÷��̾ ������ ���� (������ ������)


    [Header("Enemy")]
    public GameObject e_CountTxt; // �ܿ� �� ���� ǥ�� �ؽ�Ʈ(�ڽĵ� �ؽ�Ʈ ������ ���� ���� �θ� �޾ƿ�)
    [HideInInspector]
    public int e_Count; // �� ����

    [Header("UI")]
    public GameObject ui_InfoSetting; // ȯ�漳��â �� ����â (ȯ�漳���� Ű�� ������ ���� ���� �� ���� ����)
    public Text[] ui_ResultTxts; // ��� �ؽ�Ʈ ǥ��
    public GameObject ui_EnergyPnl; // �ܿ� �������� ǥ�����ִ� �ǳ�
    public Image[] ui_ScoreBImgs; // Ŭ����� ������ ǥ�����ִ� �̹��� (Back)
    public Image[] ui_ScoreFImgs; // Ŭ����� ������ ǥ�����ִ� �̹��� (Front) �ܻ� ȿ���� ���� Back, Front ������
    public GameObject[] ui_ResultBtn = new GameObject[2]; // ����� 0 , �������� 1 (���� ���н� or ������ é�͸� ����� / �װ� �ƴҰ�� ���� �����ϸ� ��������) ��ư ����

    [Header("SkyBox")]
    public Color32 s_StartC; // SkyBox TintColor�� ����� ���̱� ������ �׻� �ʱⰪ ������ �ʿ��� ������ (Ŭ����� SkyBox TintColor �����ϰ� �����ص�)



    void Awake()
    {
        //�ش� ���������� ������ �ҷ���, ����� ������ ���� ��� 0���� �ҷ���
        p_BestScore = PlayerPrefs.GetInt(g_Level.ToString(), 0);
        e_Count = GameObject.FindGameObjectsWithTag("Enemy").Length; // ���� ���� �����ϴ� �� ���� ã��

    }
    void Start()
    {
        ResetState();
        g_AudioSource = GetComponent<AudioSource>();
    }

    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        p_BuyCount = 0;
        p_Score = 0;
        g_State = GameState.playing;
        p_CurEnergy = p_MaxEnergy;

        RenderSettings.skybox.SetColor("_Tint", s_StartC); // SkyBox TintColor �ʱⰪ ����(��ο��)


        Text[] e_CountTxts = this.e_CountTxt.GetComponentsInChildren<Text>();
        foreach (Text text in e_CountTxts) // �ܿ� �� �ؽ�Ʈ�� ǥ��
        {
            text.text = e_Count.ToString();
        }


        for (int i = 0; i < ui_EnergyPnl.transform.childCount; i++)  // ������ �ǳ� ������ ��� ��Ȱ��ȭ
        {
            ui_EnergyPnl.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < p_CurEnergy; i++) // ������ �ǳ� ������ ��ŭ Ȱ��ȭ
        {
            ui_EnergyPnl.transform.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < u_Cost_Txt.Length; i++) // ���� ���� �ؽ�Ʈ ����
        {
            Text[] texts = u_Cost_Txt[i].GetComponentsInChildren<Text>();
            for (int k = 0; k < texts.Length; k++)
            {
                texts[k].text = u_Cost[i].ToString();
            }
        }
    }

    public void BuyUnit(int UnitType) // ���� ���޹޾Ƽ� �ش� �迭�� �ִ� ���� ����
    {
        if (p_CurEnergy < u_Cost[UnitType]) // ���� �������� ���� ���ݺ��� ������� 
        {
            g_AudioSource.clip = g_AudioClips[0];
            g_AudioSource.Play();
            return;
        }
        else // ���� �������� ���� ���ݰ� ���ų� ���� ��� (���Ű���)
        {
            p_CurEnergy -= u_Cost[UnitType]; // ������ ����
            g_AudioSource.clip = g_AudioClips[1];
            g_AudioSource.Play();
            p_BuyCount++; // ����Ƚ�� ����
            for (int i = p_MaxEnergy-1; i >= p_CurEnergy; i--) // ������ �ǳ� ǥ�ð��� ����
            {
                ui_EnergyPnl.transform.GetChild(i).gameObject.SetActive(false);
            }

            p_CurUnit = Instantiate(u_Setting[UnitType], playerCtrl.transform.position, Quaternion.identity) as GameObject; // ���� ���� �� p_CurUnit�� ������ ���� ����
            PlayerLSwitch.pLH_SelectedU = p_CurUnit.GetComponent<UnitMain>(); // pLH�� ������ ���� ����
            p_CurUnit.GetComponent<UnitMain>().u_State = UnitState.fireready; // ���ֿ� �߻� �� ���·� State����
            PlayerCtrl.p_State = PlayerState.aim; // �÷��̾� ���� ���·� ����
        }
        
    }

    public void DecreaseEnemy() // �� ���� ���� (�� �ı��� �ߵ�)
    {
        e_Count--; // �� ���� ����

        Text[] e_CountTxts = this.e_CountTxt.GetComponentsInChildren<Text>(); // �ܿ��� �ؽ�Ʈ ǥ�ð��� ����
        foreach (Text text in e_CountTxts)
        {
            text.text = e_Count.ToString();
        }

        if (e_Count <= 0) // �ܿ����� ���� ���
        {
            g_State = GameState.clear; // ���� Ŭ����

            // �÷��̾� ���� ���� (����� Ŭ����� 1�� ���� ����Ƚ�� 1�� ������ ���� 1������ �� 3�� ����)
            p_Score += 1;
            if (p_BuyCount <= g_ScoreBuyCount)
            {
                p_Score += 1;
            }
            if(p_CurEnergy >= g_ScoreEnergyCount)
            {
                p_Score += 1;
            }


            for (int i = ui_ScoreFImgs.Length; i > p_Score; i--) // ������ ���� �̹��� ������
            {
                ui_ScoreFImgs[i-1].color = new Color(0.5f, 0.5f, 0.5f);
            }

            // �ش� ������ �ִ� �������� ũ�� �÷��̾� ���� ���� (���� �ִ� ������ ������ 0���� �ڵ�������) 
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
    public void DecreaseUnit() // ������ ������鼭 ȣ�� => ������ ��� ���� Ŭ����üũ
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

    private IEnumerator ClearCheck() //���� Ŭ����� Ȯ��
    {

        yield return new WaitForSeconds(5f); //���� �ܺ� ������� ���� Ȯ���� ������ 5�ʵ� ����

        if (FadeEffect.reloadScene) // �������� (Fadeȿ���� �������϶��� �ٸ��� �̵����̱� ������ ��������)
            yield break;

        if (e_Count >= 1) // �� �ܿ��� �й� �ƴҽ� Ŭ����
        {
            g_State = GameState.fail;
        }
        else // �ܿ����� ������ 
        {
            p_Score = 1; // �������� �� �Ҹ��ϸ� ����Ƚ�� ���� �������� ���ϱ� ������ 1�� ����

            
            if (p_Score > p_BestScore) // �÷��̾� ���� ����
            {
                PlayerPrefs.SetInt(g_Level.ToString(), p_Score);
            }

            g_State = GameState.clear; // Ŭ���� ����
        }
    }



    void Update()   
    {
        if (g_State == GameState.clear) // Ŭ���� ��
        {
            if(playerCtrl.isSet) // �÷��̾ �� ��ġ�� ������ SkyBox ����� ����
            {
                RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), Color.white, Time.deltaTime * 0.6f));
            }
        }

        switch (g_State) // ���� ���¿� ���� UI ����
        {
            case GameState.playing:
                ui_InfoSetting.SetActive(false);
                break;

            case GameState.pause: // TimeScale�� 0���� �ϱ⺸�� ���ӿ� pause��� State�� ����� Ÿ �ൿ�� ���ϰ� ����
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
