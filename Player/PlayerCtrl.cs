using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState // 플레이어 상태
{
    normal, aim, fire, button //뷰 1(기본), 뷰 2(발사 준비중), 뷰 3(발사)
}


public class PlayerCtrl : MonoBehaviour // p => 플레이어 u => 유닛 
{
    [Header("Player State")]
    public static PlayerState p_State;
    [HideInInspector]
    public bool isSet; // 플레이어가 발사 준비가 됐는지 (p_PosVec과 현재 위치랑 비교)


    [Header("Player Camera Setting")]
    public float p_MaxFollowTIme = 5f; // 추적 최대 시간
    private float p_CurFollowTime = 0; // 현재 남은 추적시간
    [HideInInspector]
    public Vector3 p_PosVec; // 플레이어 초기 위치 (돌아가기 용)
    private Vector3 p_LastPosVec; // 추적시간이 지나거나 버튼 클릭한 위치를 설정해 플레이어 위치 고정
    public static GameObject u_CameraTarget; // 따라갈 유닛

    [Header("Game Mgr")]
    public GameMgr gameMgr;
    void Start()
    {
        ResetState();
    }

    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        p_State = PlayerState.normal;
        p_PosVec = this.transform.position;
        u_CameraTarget = null;
        p_CurFollowTime = p_MaxFollowTIme;
        isSet = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(transform.position != p_PosVec) // 플레이어 초기값과 현재위치 비교해 준비여부 확인
        {
            isSet = true;
        }
        else
        {
            isSet = false;
        }

        if (isSet  && OVRInput.GetDown(OVRInput.Button.One) && gameMgr.g_State== GameState.playing && p_State == PlayerState.normal) // 환경설정 버튼 클릭시 게임 멈춤
        {
            gameMgr.g_State = GameState.pause;
        }
        else if(isSet && OVRInput.GetDown(OVRInput.Button.One) && gameMgr.g_State == GameState.pause && p_State == PlayerState.normal) // 환경설정 버튼을 눌러 멈춘 상태에 다시 클릭할 경우 게임 재개
        {
            gameMgr.g_State = GameState.playing;
        }


        if (p_State == PlayerState.normal) //기본 상태일시
        {
            p_CurFollowTime = p_MaxFollowTIme; // 추적시간 초기화
            gameObject.GetComponent<OVRManager>().trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
        }
        else if (p_State == PlayerState.aim) //유닛을 구매했거나 조준중인 상태일시
        {
        }
        else if (p_State == PlayerState.fire) //발사 상태 (유닛이 날라가고 있는 경우)
        {
            if (u_CameraTarget.GetComponent<UnitMain>().p_CameraFollow) // 카메라가 유닛을 쫓아가는 상태인지 확인
            {
                if (p_CurFollowTime >= 0) // 추적시간이 유효한 경우
                {
                    p_CurFollowTime -= Time.deltaTime; // 추적시간 감소
                    transform.position = u_CameraTarget.transform.position; // 유닛위치를 따라감
                    gameObject.GetComponent<OVRManager>().trackingOriginType = OVRManager.TrackingOrigin.EyeLevel;
                    p_LastPosVec = transform.position;
                }
                else
                {
                    transform.position = p_LastPosVec;
                }
            }
            else
            {
                transform.position = p_LastPosVec;
            }
        }
    }

}
