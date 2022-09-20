using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unittrack : UnitMain
{
    public GameObject w_TrackGob; // ������ ����Ʈ
    public float w_Dmg = 50; // ������ ������
    public float w_Radius = 6; // ������ ������


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

        Instantiate(w_TrackGob, lazerPos, Quaternion.identity); // ������ ����Ʈ ���
        foreach (Collider col in Capsule) // ������ �ݶ��̴��� ���̸� �������� ����
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
