using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyMain : MonoBehaviour // e => 적 ,, 현재는 Enemy종류가 기본밖에 없으며, 추후 추가될 예정인 Enemy들은 다양한 스킬, 변수등을 포함할 수 있기 때문에 현재는 override를 고려하지 않고 작성함(적이 추가되면 변경될 예정)
{
    [Header("Enemy")]
    public float e_MaxHp = 40; // 적 최대체력
    private float e_CurHp; // 적 현재체력
    private Animator e_Ani; // 적 애니메이션
    private bool e_Death; // 죽어있는가 (Dissolve 애니메이션 효과로 즉시 사라지는게 아니기 때문에 지정)
    public Image e_StateImg = null;


    [Header("GameMgr")]
    protected GameMgr gameMgr;

    [Header("DissolveAnimation")] // 다운로드 받은 스크립트
    private U10PS_DissolveOverTime dissolve_Ani;
    void Start()
    {
        ResetState();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        e_Ani = GetComponent<Animator>();
        dissolve_Ani = GetComponentInChildren<U10PS_DissolveOverTime>();
    }


    private void ResetState() //시작시 변수 및 오브젝트 초기화
    {
        e_Death = false;
        dissolve_Ani.enabled = false;
        e_CurHp = e_MaxHp;

    }

    public void TakeDamage(float damage) // 유닛 또는 무기가 충돌시 해당 오브젝트에서 발동 , damage값만큼 피해를 입음
    {
        if (e_Death) // 죽었을 경우 무시 (애니메이션 발생하고 있는경우) 
            return;

        e_MaxHp -= damage; // 체력감소

        if (e_MaxHp > 0) // 현재 체력이 0보다 이상인경우 피격 애니메이션 출력 (구매한 에셋이기 때문에 상황과 가장 이상적인 애니메이션이 없어 Defend라는 애니메이션이 피격 애니메이션과 가장 유사하기 때문에 사용)
        {
            e_Ani.SetTrigger("Defend"); 
        }
        else // 체력이 0보다 작으면 죽음 애니메이션, 소리, 함수, 죽음 선언
        {
            GetComponent<AudioSource>().Play();
            e_Ani.SetTrigger("Die");
            Invoke("EnemyDeath", 1.5f);
            e_Death = true;
        }
    }

    private void Update()
    {

        if (e_MaxHp / e_CurHp >= 0.7f) // 체력에 비례해서 이미지 색상으로 대략적인 남은 Hp를 표시함
        {
            e_StateImg.color = new Color(0, 1, 0);


        }
        else if (e_MaxHp / e_CurHp >= 0.4f)
        {
            e_StateImg.color = new Color(1, 1, 0);

        }
        else if (e_MaxHp / e_CurHp > 0)
        {
            e_StateImg.color = new Color(1, 0, 0);
        }
        else
        {
            e_StateImg.color = new Color(0.4f, 0.4f, 0.4f);
        }
    }

    private void OnCollisionEnter(Collision collision) // 오브젝트 데미지 (바닥, 지형지물에 충돌 ex: 낙하와 같은 상황에 발동)
    {
        if (collision.collider.CompareTag("Unit") || collision.collider.CompareTag("Weapon")) // 유닛과 무기에 중첩 데미지 제외
        {
            return;
        }
        float Ob_Damaged = (collision.relativeVelocity.magnitude) / 30; //충격량
        TakeDamage(Ob_Damaged);
    }

    private void EnemyDeath() // 유닛이 죽을때 발동 애니메이션 출력후 파괴
    {
        e_StateImg.enabled = false;
        dissolve_Ani.enabled = true;
        gameMgr.DecreaseEnemy();
        Destroy(gameObject,2);
    }
}
