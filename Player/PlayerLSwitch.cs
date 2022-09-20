using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLSwitch : MonoBehaviour // pLH => 플레이어 좌측 컨트롤러, pRH => 플레이어 우측 컨트롤러, gL => 가이드라인,  ui => 유닛 구매 UI
{
    [Header("PlayerCtrl")]
    public PlayerCtrl playerCtrl;

    [Header("GameMgr")]
    public GameMgr gameMgr;

    [Header("UI")]
    public UI_SlideManager_OVR UI_SlideManager_OVR; // 하민형 담당

    [Header("L Switch")]
    public GameObject pLH_AimEffect; // 조준시 나타나는 이펙트
    public static UnitMain pLH_SelectedU; // 현재 좌측 컨트롤러에 부착된 유닛 (구매 및 관리를 위해 Static 선언)
    private float pLH_JoyStickInput; // 좌측 조이스틱 이동값 (유닛 선택용)

    [Header("R Switch")]
    public Transform pRH_Pos; // 파워값 , 좌측 컨트롤러, 우측 컨트롤러 위치를 계산해 발사값 설정용
    private PlayerRSwitch playerRswitch; 

    [Header("Warp")]
    public GameObject pLH_Warp; // 좌측 컨트롤러 스프라이트 (이펙트용)
    private float pLH_WarpPower; // 워프 스프라이트 회전값 (이펙트용)

    [Header("Power & GuideLine")] 
    public float p_Power = 500; // 플레이어 발사값의 곱해줄 정도 (능동적인 조절을 위해 public으로 선언 / 밸런스, 맵 주기적인 변경, 중력값 때문)
    public float gL_Time = 3f; // 하민형 담당 
    private LineRenderer guideLine; // 하민형 담당




    void Start()
    {
        ResetState();
        guideLine = GetComponent<LineRenderer>();
        playerRswitch = pRH_Pos.GetComponent<PlayerRSwitch>();
    }

    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        pLH_SelectedU = null;
    }


    void Update()
    {
        if(gameMgr.g_State == GameState.pause) // 멈춘상태일시 이펙트, 구매창 off
        {
            pLH_Warp.SetActive(false);
            UI_SlideManager_OVR.GetComponent<Canvas>().enabled = false;
            return;
        }
        if (PlayerCtrl.p_State == PlayerState.normal &&playerCtrl.isSet) // 플레이어가 발사가능한 상태일때
        {

            if (gameMgr.p_CurEnergy > 0 && gameMgr.e_Count>0) // 게임 진행이 가능할 때
            {
                pLH_Warp.SetActive(true); // 워프 스프라이트 온

                UI_SlideManager_OVR.U_RenderEnable(true);
                float pLH_JoyStickInputX = LeftHand_GetJoystickInput();
                UI_SlideManager_OVR.UI_Slide(pLH_JoyStickInputX); // 좌우측으로 유닛 넘김
                if (pLH_JoyStickInputX < -0.5 || pLH_JoyStickInputX > 0.5) 
                {
                    UI_SlideManager_OVR.GetComponent<Canvas>().enabled = true;

                }
                if (LeftHand_GetDownUIOpen())
                {
                    UI_SlideManager_OVR.GetComponent<Canvas>().enabled = false;
                }
            }
            else
            {
                UI_SlideManager_OVR.GetComponent<Canvas>().enabled = false;
                UI_SlideManager_OVR.U_RenderEnable(false);
            }
        }
        else if (PlayerCtrl.p_State == PlayerState.aim) // 유닛을 구매했거나 조준중인 상태일시
        {
            UI_SlideManager_OVR.U_RenderEnable(false);
            UI_SlideManager_OVR.GetComponent<Canvas>().enabled = false;

            pLH_AimEffect.SetActive(true); // 에임 이펙트 on
            pLH_Warp.SetActive(true); // 워프가 꺼져있을 수 있어 워프 스프라이트 온 (ex: 게임 중지)
            pLH_WarpPower = Mathf.Clamp(((transform.position - pRH_Pos.position).magnitude * 5f), 0, 5); // 워프 스프라이트 돌아가는 세기값 설정

            if (playerRswitch.RightHand_Grab() && playerRswitch.Getp_firereaby()) // 조준중인 상태인 경우 가이드라인, 워프 회전, 유닛 발사값 설정
            {
                GetComponent<LineRenderer>().enabled = true;
                Vector3 objVelocity = (transform.position - pRH_Pos.position) * p_Power; // velocity(발사값) =  (좌측 컨트롤러 위치 - 우측 컨트롤러 위치) *  힘  이렇게 
                DrawParabolaLine(transform.position, objVelocity, guideLine, gL_Time); // 가이드라인


                playerRswitch.u_Velocity = objVelocity; // 발사값
                pLH_Warp.transform.Rotate(new Vector3(0, 0, pLH_WarpPower)); // 워프가 돌아가도록 회전설정

            }
            else
            {
                GetComponent<LineRenderer>().enabled = false;
            }
        }
        else if (PlayerCtrl.p_State == PlayerState.fire) // 발사된 상태일때
        {
            pLH_AimEffect.SetActive(false); // 에임 이펙트 off
            pLH_Warp.SetActive(false); // 워프 스프라이트 off
        }


    }

    public void DrawParabolaLine(Vector3 startPos, Vector3 objVelocity, LineRenderer line, float time = 1f) // 중력, 위치, 힘에 맞춰 포물선 그려주는 함수
    {
        line.positionCount = (int)(50 * time);
        int frame = line.positionCount;
        Vector3 objPosition = startPos;
        line.SetPosition(0, objPosition);
        for (int i = 1; i < frame; i++) 
        {
            objVelocity += Physics.gravity * 0.02f;
            objPosition += objVelocity * 0.02f;
            line.SetPosition(i, objPosition);
        }
    }


    public bool LeftHand_GetDownUIOpen()  // 좌측 컨트롤러 구매 UI On/Off 버튼
    {
        return OVRInput.GetDown(OVRInput.Button.Three);
    }
    
    public float LeftHand_GetJoystickInput() // 좌측 컨트롤러 유닛 이동 인덱스
    {
        pLH_JoyStickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        return pLH_JoyStickInput;
    }
}
