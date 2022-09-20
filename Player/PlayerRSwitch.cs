using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;


public class PlayerRSwitch : MonoBehaviour // l => ���η�����(������), u => ����, pRH => �÷��̾� ���� ����ġ, p => �÷��̾�
{
    [Header("GameMgr")]
    public GameMgr gameMgr;

    [Header("PlayerCtrl")]
    private PlayerCtrl playerCtrl;
    private float p_WaitTime; // �� �ε�� ���ð�

    [Header("UI")]
    public UI_SlideManager_OVR UI_SlideManager_OVR; // �Ϲ��� ���

    [Header("Unit")]
    public Vector3 u_Scale; // �ν����� â�� ���� ������ ũ�Ⱚ ������ ���� Public ����
    [HideInInspector]
    public Vector3 u_Velocity; // ���ֿ��� �ο��� �ӵ���
    private UnitMain unit; // ���������� ��ũ��Ʈ ������ ���ϰ� �ϱ����� UnitMain ��ũ��Ʈ�� �޾ƿ�

    [Header("Laser")]
    public Transform l_Target; // �������� ���ϴ� ��ġ (�⺻ �����϶� ������ ��ġȮ�� ����ȭ)
    public Transform l_AimTarget; // AIm �����϶� �������� ���ϴ� ��ġ
    private LineRenderer laser; // ������(���η�����)

    [Header("L Switch Pos")]
    public Transform pLH_FireTrs; // ���� ��Ʈ�ѷ� ��ġ

    [Header("R Switch Vib & Effect")]
    public GameObject pRH_ParticleGob; // ����Ʈ ���� ���ӿ�����Ʈ
    public GameObject pRH_ParticleL; // �������� ȸ���ϴ� ����Ʈ
    public GameObject pRH_ParticleR; // �������� ȸ���ϴ� ����Ʈ
    private float pRH_VibPower; // ���� ����
    

    [Header("Fire Ready")]
    private bool p_fireready; // �߻� ���� ����
    public bool Getp_firereaby() { return p_fireready; } 



    void Start()
    {
        ResetState();
        playerCtrl = GetComponentInParent<PlayerCtrl>();
        GetComponent<LineRenderer>().enabled = true;
        laser = this.gameObject.GetComponent<LineRenderer>();
        
    }
    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        unit = null;
        p_fireready = false;
        p_WaitTime = 3.0f;
    } 

    void Update()
    {
        if (p_WaitTime >= 0) // ���ð� ����
        {
            p_WaitTime -= Time.deltaTime;
            return;
        }

        laser.SetPosition(0, transform.position);


        if (PlayerCtrl.p_State == PlayerState.normal &&playerCtrl.isSet) // �÷��̾ �Ϲ� & ���Ű� ������ ����
        {
            pRH_ParticleGob.SetActive(true);
            laser.SetPosition(1, l_Target.position);
            laser.enabled = true;

        
            if (RightHand_GetGrab()) // ���� ��Ʈ�ѷ� Ʈ���Ÿ� ������
            {
                if(gameMgr.g_State != GameState.playing) // �������� �ƴ� ���� ex: ���� ����, ���� ��
                {
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit)) // ���� ��Ʈ�ѷ� ������ ���� �߻�
                    {
                        if (hit.collider.CompareTag("Button")) // ��ư�� ������� �ش� ��ư Ŭ��
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
                    if(CanBuyUnit()) // �������� �� ��Ʈ�ѷ��� ������ ������ ���� ������ ������ ��� ������ ���� �� ���� ���·� ����(���� ���´� ���� ���� GameMgr���� ����)
                    {
                        gameMgr.BuyUnit(UI_SlideManager_OVR.u_ImagesIndex);
                        unit = PlayerLSwitch.pLH_SelectedU; // �ش� ������ �߻� �������� ����
                        unit.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                        p_fireready = true; // �߻� �غ�Ϸ�
                    }
                }

            }
        }
        else if (PlayerCtrl.p_State == PlayerState.aim) // ���� ������ ���
        {

            if (p_fireready && RightHand_Grab()) // ���� ���¿��� Ʈ���Ÿ� ������ ���� ���
            {
                laser.enabled = true;
                unit.transform.position = pLH_FireTrs.position; // ���� ��ġ�� ���� ��Ʈ�ѷ��� ���߾� �̵�
                laser.SetPosition(1, l_AimTarget.position); // �������� ������ ��ġ�� ������
                pRH_VibPower = Mathf.Clamp(((this.transform.position - pLH_FireTrs.position).magnitude), 0.0f, 0.8f) / 2.5f; // ��,���� ��Ʈ�ѷ��� ��ġ�� ����� �������� ���
                pRH_ParticleL.transform.Rotate(0, 0, -5); // ����Ʈ�� �������� ȸ����Ŵ
                pRH_ParticleR.transform.Rotate(0, 0, 5); // ����Ʈ�� �������� ȸ����Ŵ
                
                if (SoundVibOnOff.vibOn) // ���� Off���°� �ƴҰ�� ���� �������� ��ŭ ���� ��Ʈ�ѷ��� ����ȿ�� �ο�
                    OVRInput.SetControllerVibration(pRH_VibPower, pRH_VibPower, OVRInput.Controller.RTouch);
            }

            if (RightHand_OffGrab() && p_fireready) // ���� ���¿��� Ʈ���Ÿ� ���� ���
            {
                GetComponent<AudioSource>().Stop(); // ���� ����
                laser.enabled = false; 
                pRH_ParticleGob.SetActive(false); // ��ƼŬ ���� ��Ȱ��ȭ

                unit.GetComponent<Rigidbody>().velocity = u_Velocity; // ���ֿ� �ӵ� �ο�
                unit.u_State = UnitState.fire; // ���ֻ��� ����

                OVRInput.SetControllerVibration(0.0f, 0.0f, OVRInput.Controller.RTouch); // ���� Off

                PlayerCtrl.p_State = PlayerState.fire; // �÷��̾� State�� ���󰡴� ���·� ����
                p_fireready = false; // �غ�Ϸ� ��Ȱ��ȭ
            }
        }
        else if (PlayerCtrl.p_State == PlayerState.fire) // ������ ���󰡴� ���϶� 
        {
            unit.transform.localScale = u_Scale; // ���� ���ٰ����� ���� ����ߴ� ���� ����� ���� ������� �ٽ� ����

            if (unit != null) // ������ ���� ������ ���
            {
                if (unit.u_SkillAvailable) // ��ų�� ��� ������ ������ ���
                {
                    if (RightHand_GetGrab()) // ù Ʈ���� ������ �÷��̾�(ī�޶�) ���� ����
                    {
                        unit.p_CameraFollow = false;
                    }
                    if (RightHand_OffGrab()) // Ʈ���Ÿ� ���� ������ ��ų�� ���
                    {
                        unit.UseSkill();
                    }
                }
                else
                {
                    if (RightHand_GetGrab()) // ��ų�� ��� �Ұ����� ������ ��� ������ ����
                    {
                        unit.p_CameraFollow = false;
                    }
                }

            }

        }
    }

    public bool CanBuyUnit() // �߻� ������ ��Ʈ�ѷ� ��ġ & ������ �����ϱ� ���� �������� ���� ��� True ��ȯ
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

    public bool RightHand_GetGrab()  // ���� ��Ʈ�ѷ� Ʈ���Ÿ� ������
    {
        return OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
    }
    public bool RightHand_OffGrab() // ���� ��Ʈ�ѷ� Ʈ���Ÿ� ����
    {
        return OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);
    }
    public bool RightHand_Grab()  // ���� ��Ʈ�ѷ� Ʈ���� ��ü���� ����
    {
        return OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
    }
}
