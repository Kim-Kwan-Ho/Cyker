using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFlyEffect : MonoBehaviour //���ֿ� ������!
{
    public GameObject U_FlyingEffectPrefab; //���󰡴� ����Ʈ ������
    public float U_EffectSpeedLimit = 5; //������ �����ӵ����Ϸ� �������� ����Ʈ�� ������

    ParticleSystem.MainModule flyingEffectModule;
    UnitMain unitMain;
    GameObject flyingEffect;

    private void Start()
    {
        flyingEffect = Instantiate(U_FlyingEffectPrefab, transform.position, Quaternion.identity);
        flyingEffect.transform.parent = gameObject.transform; //���� ������Ʈ�� �ڽ����� ����
        flyingEffectModule = flyingEffect.GetComponent<ParticleSystem>().main;
        flyingEffect.SetActive(false);
        unitMain = gameObject.GetComponent<UnitMain>();
    }

    private void Update()
    {
        if (unitMain.u_State == UnitState.fire && unitMain.p_CameraFollow == true) //�߻����϶�
        {
            flyingEffect.SetActive(true);
            flyingEffectModule.loop = true;

            Vector3 dir = gameObject.GetComponent<Rigidbody>().velocity;
            if (dir.magnitude != 0) // �ӵ����� 0�� �ƴҶ�
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized); //���ֿ�����Ʈ�� ���ư��� ������ �ٶ󺸰� ȸ��
                flyingEffect.transform.rotation = rot;
            }

            if (gameObject.GetComponent<Rigidbody>().velocity.magnitude < U_EffectSpeedLimit)
            {
                flyingEffectModule.loop = false;
            }
        }
        else
        {
            flyingEffect.SetActive(false);
        }
    }
}
