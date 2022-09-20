using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unittrack : UnitMain
{
    public GameObject w_TrackGob; // 레이저 이펙트
    public float w_Dmg = 50; // 레이저 데미지
    public float w_Radius = 6; // 레이저 반지름


    public override void UseSkill()
    {
        base.UseSkill();
        if (SoundVibOnOff.vibOn == true&& u_IsVibe == false)
        {
            StartCoroutine(VibrationAfterSkillUse());
            u_IsVibe = true;
        }

        Vector3 lazerPos = transform.position;
        lazerPos.y = 0;
        Collider[] Capsule = Physics.OverlapCapsule(lazerPos, lazerPos + Vector3.up*500, w_Radius);

        Instantiate(w_TrackGob, lazerPos, Quaternion.identity); // 레이저 이펙트 출력
        foreach (Collider col in Capsule) // 추출한 콜라이더가 적이면 데미지를 입힘
        {
            if (col != null)
            {
                if (col.CompareTag("Enemy"))
                {
                    col.GetComponent<EnemyMain>().TakeDamage(w_Dmg);
                }
            }
        }
    }
}
