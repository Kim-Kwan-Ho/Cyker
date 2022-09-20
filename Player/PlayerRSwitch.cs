using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;


public class PlayerRSwitch : MonoBehaviour // l => 라인렌더러(레이저), u => 유닛, pRH => 플레이어 우측 스위치, p => 플레이어
{
    [Header("GameMgr")]
    public GameMgr gameMgr;

    [Header("PlayerCtrl")]
    private PlayerCtrl playerCtrl;
    private float p_WaitTime; // 씬 로드시 대기시간

    [Header("UI")]
    public UI_SlideManager_OVR UI_SlideManager_OVR; // 하민형 담당

    [Header("Unit")]
    public Vector3 u_Scale; // 인스펙터 창을 통한 유연한 크기값 변동을 위해 Public 선언
    [HideInInspector]
    public Vector3 u_Velocity; // 유닛에게 부여할 속도값
    private UnitMain unit; // 유닛이지만 스크립트 접근을 편하게 하기위해 UnitMain 스크립트로 받아옴

    [Header("Laser")]
    public Transform l_Target; // 레이저가 향하는 위치 (기본 상태일때 포인터 위치확인 간편화)
    public Transform l_AimTarget; // AIm 상태일때 레이저가 향하는 위치
    private LineRenderer laser; // 레이저(라인렌더러)

    [Header("L Switch Pos")]
    public Transform pLH_FireTrs; // 좌측 컨트롤러 위치

    [Header("R Switch Vib & Effect")]
    public GameObject pRH_ParticleGob; // 이펙트 상위 게임오브젝트
    public GameObject pRH_ParticleL; // 좌측으로 회전하는 이펙트
    public GameObject pRH_ParticleR; // 우측으로 회전하는 이펙트
    private float pRH_VibPower; // 진동 세기
    

    [Header("Fire Ready")]
    private bool p_fireready; // 발사 가능 여부
    public bool Getp_firereaby() { return p_fireready; } 



    void Start()
    {
        ResetState();
        playerCtrl = GetComponentInParent<PlayerCtrl>();
        GetComponent<LineRenderer>().enabled = true;
        laser = this.gameObject.GetComponent<LineRenderer>();
        
    }
    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        unit = null;
        p_fireready = false;
        p_WaitTime = 3.0f;
    } 

    void Update()
    {
        if (p_WaitTime >= 0) // 대기시간 설정
        {
            p_WaitTime -= Time.deltaTime;
            return;
        }

        laser.SetPosition(0, transform.position);


        if (PlayerCtrl.p_State == PlayerState.normal &&playerCtrl.isSet) // 플레이어가 일반 & 구매가 가능한 상태
        {
            pRH_ParticleGob.SetActive(true);
            laser.SetPosition(1, l_Target.position);
            laser.enabled = true;

        
            if (RightHand_GetGrab()) // 우측 컨트롤러 트리거를 누를시
            {
                if(gameMgr.g_State != GameState.playing) // 게임중이 아닌 상태 ex: 게임 종료, 정지 등
                {
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit)) // 우측 컨트롤러 앞으로 레이 발사
                    {
                        if (hit.collider.CompareTag("Button")) // 버튼에 맞을경우 해당 버튼 클릭
                        {
                            UiBtn btn = hit.collider.GetComponent<UiBtn>();
                            PlayerCtrl.p_State = PlayerState.button;
                            btn.ClickBtn();
                            laser.enabled = false;
                        }
                    }
                }
                else
                {
                    if(CanBuyUnit()) // 게임중일 때 컨트롤러가 지정된 유닛을 구매 가능한 상태일 경우 유닛을 구매 후 조준 상태로 변경(조준 상태는 유닛 구매 GameMgr에서 설정)
                    {
                        gameMgr.BuyUnit(UI_SlideManager_OVR.u_ImagesIndex);
                        unit = PlayerLSwitch.pLH_SelectedU; // 해당 유닛을 발사 유닛으로 설정
                        unit.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                        p_fireready = true; // 발사 준비완료
                    }
                }

            }
        }
        else if (PlayerCtrl.p_State == PlayerState.aim) // 조준 상태일 경우
        {

            if (p_fireready && RightHand_Grab()) // 조준 상태에서 트리거를 누르고 있을 경우
            {
                laser.enabled = true;
                unit.transform.position = pLH_FireTrs.position; // 유닛 위치를 좌측 컨트롤러에 맞추어 이동
                laser.SetPosition(1, l_AimTarget.position); // 레이저를 설정한 위치로 포인팅
                pRH_VibPower = Mathf.Clamp(((this.transform.position - pLH_FireTrs.position).magnitude), 0.0f, 0.8f) / 2.5f; // 좌,우측 컨트롤러의 위치에 비례해 진동세기 계산
                pRH_ParticleL.transform.Rotate(0, 0, -5); // 이펙트를 좌측으로 회전시킴
                pRH_ParticleR.transform.Rotate(0, 0, 5); // 이펙트를 우측으로 회전시킴
                
                if (SoundVibOnOff.vibOn) // 진동 Off상태가 아닐경우 계산된 진동세기 만큼 우측 컨트롤러에 진동효과 부여
                    OVRInput.SetControllerVibration(pRH_VibPower, pRH_VibPower, OVRInput.Controller.RTouch);
            }

            if (RightHand_OffGrab() && p_fireready) // 조준 상태에서 트리거를 땠을 경우
            {
                GetComponent<AudioSource>().Stop(); // 사운드 멈춤
                laser.enabled = false; 
                pRH_ParticleGob.SetActive(false); // 파티클 전부 비활성화

                unit.GetComponent<Rigidbody>().velocity = u_Velocity; // 유닛에 속도 부여
                unit.u_State = UnitState.fire; // 유닛상태 변경

                OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch); // 진동 Off

                PlayerCtrl.p_State = PlayerState.fire; // 플레이어 State를 날라가는 상태로 변경
                p_fireready = false; // 준비완료 비활성화
            }
        }
        else if (PlayerCtrl.p_State == PlayerState.fire) // 유닛이 날라가는 중일때 
        {
            unit.transform.localScale = u_Scale; // 기존 원근감으로 인해 축소했던 유닛 사이즈를 기존 사이즈로 다시 변경

            if (unit != null) // 유닛이 아직 존재할 경우
            {
                if (unit.u_SkillAvailable) // 스킬이 사용 가능한 유닛인 경우
                {
                    if (RightHand_GetGrab()) // 첫 트리거 누를시 플레이어(카메라) 추적 멈춤
                    {
                        unit.p_CameraFollow = false;
                    }
                    if (RightHand_OffGrab()) // 트리거를 뗄시 유닛의 스킬을 사용
                    {
                        unit.UseSkill();
                    }
                }
                else
                {
                    if (RightHand_GetGrab()) // 스킬이 사용 불가능한 유닛인 경우 추적만 멈춤
                    {
                        unit.p_CameraFollow = false;
                    }
                }

            }

        }
    }

    public bool CanBuyUnit() // 발사 가능한 컨트롤러 위치 & 유닛을 구매하기 위한 에너지가 있을 경우 True 반환
    {
        if(gameMgr.p_CurEnergy >= gameMgr.u_Cost[UI_SlideManager_OVR.u_ImagesIndex] && Mathf.Abs(Vector3.Distance(transform.position, pLH_FireTrs.position)) < 0.15) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RightHand_GetGrab()  // 우측 컨트롤러 트리거를 누를시
    {
        return OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
    }
    public bool RightHand_OffGrab() // 우측 컨트롤러 트리거를 뗄시
    {
        return OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);
    }
    public bool RightHand_Grab()  // 우측 컨트롤러 트리거 전체적인 여부
    {
        return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
    }
}
