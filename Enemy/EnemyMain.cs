using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyMain : MonoBehaviour // e => �� ,, ����� Enemy������ �⺻�ۿ� ������, ���� �߰��� ������ Enemy���� �پ��� ��ų, �������� ������ �� �ֱ� ������ ����� override�� ������� �ʰ� �ۼ���(���� �߰��Ǹ� ����� ����)
{
    [Header("Enemy")]
    public float e_MaxHp = 40; // �� �ִ�ü��
    private float e_CurHp; // �� ����ü��
    private Animator e_Ani; // �� �ִϸ��̼�
    private bool e_Death; // �׾��ִ°� (Dissolve �ִϸ��̼� ȿ���� ��� ������°� �ƴϱ� ������ ����)
    public Image e_StateImg = null;


    [Header("GameMgr")]
    protected GameMgr gameMgr;

    [Header("DissolveAnimation")] // �ٿ�ε� ���� ��ũ��Ʈ
    private U10PS_DissolveOverTime dissolve_Ani;
    void Start()
    {
        ResetState();
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        e_Ani = GetComponent<Animator>();
        dissolve_Ani = GetComponentInChildren<U10PS_DissolveOverTime>();
    }


    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        e_Death = false;
        dissolve_Ani.enabled = false;
        e_CurHp = e_MaxHp;

    }

    public void TakeDamage(float damage) // ���� �Ǵ� ���Ⱑ �浹�� �ش� ������Ʈ���� �ߵ� , damage����ŭ ���ظ� ����
    {
        if (e_Death) // �׾��� ��� ���� (�ִϸ��̼� �߻��ϰ� �ִ°��) 
            return;

        e_MaxHp -= damage; // ü�°���

        if (e_MaxHp > 0) // ���� ü���� 0���� �̻��ΰ�� �ǰ� �ִϸ��̼� ��� (������ �����̱� ������ ��Ȳ�� ���� �̻����� �ִϸ��̼��� ���� Defend��� �ִϸ��̼��� �ǰ� �ִϸ��̼ǰ� ���� �����ϱ� ������ ���)
        {
            e_Ani.SetTrigger("Defend"); 
        }
        else // ü���� 0���� ������ ���� �ִϸ��̼�, �Ҹ�, �Լ�, ���� ����
        {
            GetComponent<AudioSource>().Play();
            e_Ani.SetTrigger("Die");
            Invoke("EnemyDeath", 1.5f);
            e_Death = true;
        }
    }

    private void Update()
    {

        if (e_MaxHp / e_CurHp >= 0.7f) // ü�¿� ����ؼ� �̹��� �������� �뷫���� ���� Hp�� ǥ����
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

    private void OnCollisionEnter(Collision collision) // ������Ʈ ������ (�ٴ�, ���������� �浹 ex: ���Ͽ� ���� ��Ȳ�� �ߵ�)
    {
        if (collision.collider.CompareTag("Unit") || collision.collider.CompareTag("Weapon")) // ���ְ� ���⿡ ��ø ������ ����
        {
            return;
        }
        float Ob_Damaged = (collision.relativeVelocity.magnitude) / 30; //��ݷ�
        TakeDamage(Ob_Damaged);
    }

    private void EnemyDeath() // ������ ������ �ߵ� �ִϸ��̼� ����� �ı�
    {
        e_StateImg.enabled = false;
        dissolve_Ani.enabled = true;
        gameMgr.DecreaseEnemy();
        Destroy(gameObject,2);
    }
}
