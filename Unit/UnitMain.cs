using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UnitState // 유닛 상태 기본, 발사 대기중, 날라가는중
{
    standby, fireready, fire
}


public class UnitMain : MonoBehaviour // u => 유닛, p => 플레이어
{
    [Header("Player")]
    [HideInInspector]
    public bool p_CameraFollow; // 카메라가 유닛을 쫓아오도록 하는지 여부


    [Header("GameMgr")]
    private GameMgr gameMgr;

    [Header("Unit")]
    public UnitState u_State; // 유닛 상태
    protected float u_LifeTime = 10.0f; //생존시간 10초 설정
    public bool u_SkillAvailable; // 유닛 스킬사용 가능여부
    public float u_Damage = 5; //유닛 직접 충돌 데미지 (5는 임의값)
    protected bool u_IsVibe; // 진동중인 경우 (코루틴 중복방지)
    public GameObject u_ColParticle; // 바닥과 충돌시 나타나는 이펙트
    private bool u_Sound; // 유닛이 날라가는 효과음
    private MeshRenderer[] u_Meshes; // 유닛과 유닛의 자식들의 메쉬(모양) / 유닛이 스킬이 사용하면 유닛 본체가 사라지기 때문에 비활성화 용으로 가져옴



    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        u_Meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        ResetState();
    }
    void ResetState()  //시작시 변수 및 오브젝트 초기화
    {
        u_State = UnitState.standby;
        u_Sound = true;
        u_IsVibe = false;
        p_CameraFollow = true;

    }
    void Update()
    {
        if (u_State == UnitState.fireready || u_State == UnitState.standby) // 발사전 상태일 경우 중력값, 속도, 트레일러 효과 제거 (중력값이 설정되어 있으면 떠있는 상태로 취급해 속도가 늘어남 + 속도를 0으로 지정해놓아 오류를 2차적으로 방지) 
        {
            this.GetComponent<TrailRenderer>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;

        }
        else if (u_State == UnitState.fire) // 유닛이 날라가는 중이면 중력, 트레일러 효과 활성화
        {
            PlayerCtrl.u_CameraTarget = this.gameObject; // 해당 유닛을 카메라 타겟으로 지정함 

            GetComponent<TrailRenderer>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<AudioSource>().enabled = u_Sound;


            u_LifeTime -= Time.deltaTime; 
            if (u_LifeTime < 0) // 유닛 생명시간이 다 되면 유닛 감소 함수 하출 & 플레이어 정상화와 해당 유닛 삭제
            {

                gameMgr.DecreaseUnit();
                PlayerCtrl.p_State = PlayerState.normal;
                PlayerCtrl.u_CameraTarget = null;
                Destroy(this.gameObject);
            }
        }
    }


    public virtual void UseSkill() // 스킬사용
    {
        if (u_SkillAvailable == false) // 스킬 사용 불가할 경우 캔슬
            return;

        u_SkillAvailable = false; // 스킬 사용 불가로 설정
        u_LifeTime = 2.5f; // 생명주기 연장 혹은 감소
        u_Sound = false; // 소리 비활성화

        for(int i=0; i<u_Meshes.Length; i++) // 매쉬(모양) 비활성화
        {
            u_Meshes[i].enabled = false;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().enabled = false;

        if (PlayerCtrl.p_State == PlayerState.fire) // 충돌시 카메라 따라가기 멈춤
        {
            p_CameraFollow = false;

        }
        if (collision.collider.CompareTag("Enemy")) // 적과 충돌시 적에게 데미지 , 바닥에 충돌시 충돌 이펙트 발생
        {
            collision.collider.GetComponent<EnemyMain>().TakeDamage(u_Damage);
        }
        else if (collision.collider.CompareTag("Plane"))
        {
            Instantiate(u_ColParticle, transform.position, Quaternion.identity);
        }
    }
    public IEnumerator VibrationAfterSkillUse()
    {
        OVRInput.SetControllerVibration(0.4f, 0.4f, OVRInput.Controller.Touch);
        yield return new WaitForSeconds(0.2f);
        OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.Touch);
    }

}
