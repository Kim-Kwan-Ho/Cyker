using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFlyEffect : MonoBehaviour //유닛에 넣을것!
{
    public GameObject U_FlyingEffectPrefab; //날라가는 이펙트 프리펩
    public float U_EffectSpeedLimit = 5; //유닛이 일정속도이하로 떨어지면 이펙트를 끄게함

    ParticleSystem.MainModule flyingEffectModule;
    UnitMain unitMain;
    GameObject flyingEffect;

    private void Start()
    {
        flyingEffect = Instantiate(U_FlyingEffectPrefab, transform.position, Quaternion.identity);
        flyingEffect.transform.parent = gameObject.transform; //현재 오브젝트의 자식으로 지정
        flyingEffectModule = flyingEffect.GetComponent<ParticleSystem>().main;
        flyingEffect.SetActive(false);
        unitMain = gameObject.GetComponent<UnitMain>();
    }

    private void Update()
    {
        if (unitMain.u_State == UnitState.fire && unitMain.p_CameraFollow == true) //발사중일때
        {
            flyingEffect.SetActive(true);
            flyingEffectModule.loop = true;

            Vector3 dir = gameObject.GetComponent<Rigidbody>().velocity;
            if (dir.magnitude != 0) // 속도값이 0이 아닐때
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized); //유닛오브젝트가 날아가는 방향을 바라보게 회전
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
