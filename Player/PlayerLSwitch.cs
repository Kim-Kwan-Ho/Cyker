using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLSwitch : MonoBehaviour // pLH => �÷��̾� ���� ��Ʈ�ѷ�, pRH => �÷��̾� ���� ��Ʈ�ѷ�, gL => ���̵����,  ui => ���� ���� UI
{
    [Header("PlayerCtrl")]
    public PlayerCtrl playerCtrl;

    [Header("GameMgr")]
    public GameMgr gameMgr;

    [Header("UI")]
    public UI_SlideManager_OVR UI_SlideManager_OVR; // �Ϲ��� ���

    [Header("L Switch")]
    public GameObject pLH_AimEffect; // ���ؽ� ��Ÿ���� ����Ʈ
    public static UnitMain pLH_SelectedU; // ���� ���� ��Ʈ�ѷ��� ������ ���� (���� �� ������ ���� Static ����)
    private float pLH_JoyStickInput; // ���� ���̽�ƽ �̵��� (���� ���ÿ�)

    [Header("R Switch")]
    public Transform pRH_Pos; // �Ŀ��� , ���� ��Ʈ�ѷ�, ���� ��Ʈ�ѷ� ��ġ�� ����� �߻簪 ������
    private PlayerRSwitch playerRswitch; 

    [Header("Warp")]
    public GameObject pLH_Warp; // ���� ��Ʈ�ѷ� ��������Ʈ (����Ʈ��)
    private float pLH_WarpPower; // ���� ��������Ʈ ȸ���� (����Ʈ��)

    [Header("Power & GuideLine")] 
    public float p_Power = 500; // �÷��̾� �߻簪�� ������ ���� (�ɵ����� ������ ���� public���� ���� / �뷱��, �� �ֱ����� ����, �߷°� ����)
    public float gL_Time = 3f; // �Ϲ��� ��� 
    private LineRenderer guideLine; // �Ϲ��� ���




    void Start()
    {
        ResetState();
        guideLine = GetComponent<LineRenderer>();
        playerRswitch = pRH_Pos.GetComponent<PlayerRSwitch>();
    }

    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        pLH_SelectedU = null;
    }


    void Update()
    {
        if(gameMgr.g_State == GameState.pause) // ��������Ͻ� ����Ʈ, ����â off
        {
            pLH_Warp.SetActive(false);
            UI_SlideManager_OVR.GetComponent<Canvas>().enabled = false;
            return;
        }
        if (PlayerCtrl.p_State == PlayerState.normal &&playerCtrl.isSet) // �÷��̾ �߻簡���� �����϶�
        {

            if (gameMgr.p_CurEnergy > 0 && gameMgr.e_Count>0) // ���� ������ ������ ��
            {
                pLH_Warp.SetActive(true); // ���� ��������Ʈ ��

                UI_SlideManager_OVR.U_RenderEnable(true);
                float pLH_JoyStickInputX = LeftHand_GetJoystickInput();
                UI_SlideManager_OVR.UI_Slide(pLH_JoyStickInputX); // �¿������� ���� �ѱ�
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
        else if (PlayerCtrl.p_State == PlayerState.aim) // ������ �����߰ų� �������� �����Ͻ�
        {
            UI_SlideManager_OVR.U_RenderEnable(false);
            UI_SlideManager_OVR.GetComponent<Canvas>().enabled = false;

            pLH_AimEffect.SetActive(true); // ���� ����Ʈ on
            pLH_Warp.SetActive(true); // ������ �������� �� �־� ���� ��������Ʈ �� (ex: ���� ����)
            pLH_WarpPower = Mathf.Clamp(((transform.position - pRH_Pos.position).magnitude * 5f), 0, 5); // ���� ��������Ʈ ���ư��� ���Ⱚ ����

            if (playerRswitch.RightHand_Grab() && playerRswitch.Getp_firereaby()) // �������� ������ ��� ���̵����, ���� ȸ��, ���� �߻簪 ����
            {
                GetComponent<LineRenderer>().enabled = true;
                Vector3 objVelocity = (transform.position - pRH_Pos.position) * p_Power; // velocity(�߻簪) =  (���� ��Ʈ�ѷ� ��ġ - ���� ��Ʈ�ѷ� ��ġ) *  ��  �̷��� 
                DrawParabolaLine(transform.position, objVelocity, guideLine, gL_Time); // ���̵����


                playerRswitch.u_Velocity = objVelocity; // �߻簪
                pLH_Warp.transform.Rotate(new Vector3(0, 0, pLH_WarpPower)); // ������ ���ư����� ȸ������

            }
            else
            {
                GetComponent<LineRenderer>().enabled = false;
            }
        }
        else if (PlayerCtrl.p_State == PlayerState.fire) // �߻�� �����϶�
        {
            pLH_AimEffect.SetActive(false); // ���� ����Ʈ off
            pLH_Warp.SetActive(false); // ���� ��������Ʈ off
        }


    }

    public void DrawParabolaLine(Vector3 startPos, Vector3 objVelocity, LineRenderer line, float time = 1f) // �߷�, ��ġ, ���� ���� ������ �׷��ִ� �Լ�
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


    public bool LeftHand_GetDownUIOpen()  // ���� ��Ʈ�ѷ� ���� UI On/Off ��ư
    {
        return OVRInput.GetDown(OVRInput.Button.Three);
    }
    
    public float LeftHand_GetJoystickInput() // ���� ��Ʈ�ѷ� ���� �̵� �ε���
    {
        pLH_JoyStickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        return pLH_JoyStickInput;
    }
}
