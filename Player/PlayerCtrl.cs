using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState // �÷��̾� ����
{
    normal, aim, fire, button //�� 1(�⺻), �� 2(�߻� �غ���), �� 3(�߻�)
}


public class PlayerCtrl : MonoBehaviour // p => �÷��̾� u => ���� 
{
    [Header("Player State")]
    public static PlayerState p_State;
    [HideInInspector]
    public bool isSet; // �÷��̾ �߻� �غ� �ƴ��� (p_PosVec�� ���� ��ġ�� ��)


    [Header("Player Camera Setting")]
    public float p_MaxFollowTIme = 5f; // ���� �ִ� �ð�
    private float p_CurFollowTime = 0; // ���� ���� �����ð�
    [HideInInspector]
    public Vector3 p_PosVec; // �÷��̾� �ʱ� ��ġ (���ư��� ��)
    private Vector3 p_LastPosVec; // �����ð��� �����ų� ��ư Ŭ���� ��ġ�� ������ �÷��̾� ��ġ ����
    public static GameObject u_CameraTarget; // ���� ����

    [Header("Game Mgr")]
    public GameMgr gameMgr;
    void Start()
    {
        ResetState();
    }

    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
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
        if(transform.position != p_PosVec) // �÷��̾� �ʱⰪ�� ������ġ ���� �غ񿩺� Ȯ��
        {
            isSet = true;
        }
        else
        {
            isSet = false;
        }

        if (isSet  && OVRInput.GetDown(OVRInput.Button.One) && gameMgr.g_State== GameState.playing && p_State == PlayerState.normal) // ȯ�漳�� ��ư Ŭ���� ���� ����
        {
            gameMgr.g_State = GameState.pause;
        }
        else if(isSet && OVRInput.GetDown(OVRInput.Button.One) && gameMgr.g_State == GameState.pause && p_State == PlayerState.normal) // ȯ�漳�� ��ư�� ���� ���� ���¿� �ٽ� Ŭ���� ��� ���� �簳
        {
            gameMgr.g_State = GameState.playing;
        }


        if (p_State == PlayerState.normal) //�⺻ �����Ͻ�
        {
            p_CurFollowTime = p_MaxFollowTIme; // �����ð� �ʱ�ȭ
            gameObject.GetComponent<OVRManager>().trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
        }
        else if (p_State == PlayerState.aim) //������ �����߰ų� �������� �����Ͻ�
        {
        }
        else if (p_State == PlayerState.fire) //�߻� ���� (������ ���󰡰� �ִ� ���)
        {
            if (u_CameraTarget.GetComponent<UnitMain>().p_CameraFollow) // ī�޶� ������ �Ѿư��� �������� Ȯ��
            {
                if (p_CurFollowTime >= 0) // �����ð��� ��ȿ�� ���
                {
                    p_CurFollowTime -= Time.deltaTime; // �����ð� ����
                    transform.position = u_CameraTarget.transform.position; // ������ġ�� ����
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
