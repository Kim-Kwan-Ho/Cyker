using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UnitState // ���� ���� �⺻, �߻� �����, ���󰡴���
{
    standby, fireready, fire
}


public class UnitMain : MonoBehaviour // u => ����, p => �÷��̾�
{
    [Header("Player")]
    [HideInInspector]
    public bool p_CameraFollow; // ī�޶� ������ �Ѿƿ����� �ϴ��� ����


    [Header("GameMgr")]
    private GameMgr gameMgr;

    [Header("Unit")]
    public UnitState u_State; // ���� ����
    protected float u_LifeTime = 10.0f; //�����ð� 10�� ����
    public bool u_SkillAvailable; // ���� ��ų��� ���ɿ���
    public float u_Damage = 5; //���� ���� �浹 ������ (5�� ���ǰ�)
    protected bool u_IsVibe; // �������� ��� (�ڷ�ƾ �ߺ�����)
    public GameObject u_ColParticle; // �ٴڰ� �浹�� ��Ÿ���� ����Ʈ
    private bool u_Sound; // ������ ���󰡴� ȿ����
    private MeshRenderer[] u_Meshes; // ���ְ� ������ �ڽĵ��� �޽�(���) / ������ ��ų�� ����ϸ� ���� ��ü�� ������� ������ ��Ȱ��ȭ ������ ������



    void Start()
    {
        gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
        u_Meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        ResetState();
    }
    void ResetState()  //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        u_State = UnitState.standby;
        u_Sound = true;
        u_IsVibe = false;
        p_CameraFollow = true;

    }
    void Update()
    {
        if (u_State == UnitState.fireready || u_State == UnitState.standby) // �߻��� ������ ��� �߷°�, �ӵ�, Ʈ���Ϸ� ȿ�� ���� (�߷°��� �����Ǿ� ������ ���ִ� ���·� ����� �ӵ��� �þ + �ӵ��� 0���� �����س��� ������ 2�������� ����) 
        {
            this.GetComponent<TrailRenderer>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;

        }
        else if (u_State == UnitState.fire) // ������ ���󰡴� ���̸� �߷�, Ʈ���Ϸ� ȿ�� Ȱ��ȭ
        {
            PlayerCtrl.u_CameraTarget = this.gameObject; // �ش� ������ ī�޶� Ÿ������ ������ 

            GetComponent<TrailRenderer>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<AudioSource>().enabled = u_Sound;


            u_LifeTime -= Time.deltaTime; 
            if (u_LifeTime < 0) // ���� ����ð��� �� �Ǹ� ���� ���� �Լ� ���� & �÷��̾� ����ȭ�� �ش� ���� ����
            {

                gameMgr.DecreaseUnit();
                PlayerCtrl.p_State = PlayerState.normal;
                PlayerCtrl.u_CameraTarget = null;
                Destroy(this.gameObject);
            }
        }
    }


    public virtual void UseSkill() // ��ų���
    {
        if (u_SkillAvailable == false) // ��ų ��� �Ұ��� ��� ĵ��
            return;

        u_SkillAvailable = false; // ��ų ��� �Ұ��� ����
        u_LifeTime = 2.5f; // �����ֱ� ���� Ȥ�� ����
        u_Sound = false; // �Ҹ� ��Ȱ��ȭ

        for(int i=0; i<u_Meshes.Length; i++) // �Ž�(���) ��Ȱ��ȭ
        {
            u_Meshes[i].enabled = false;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<AudioSource>().enabled = false;

        if (PlayerCtrl.p_State == PlayerState.fire) // �浹�� ī�޶� ���󰡱� ����
        {
            p_CameraFollow = false;

        }
        if (collision.collider.CompareTag("Enemy")) // ���� �浹�� ������ ������ , �ٴڿ� �浹�� �浹 ����Ʈ �߻�
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
