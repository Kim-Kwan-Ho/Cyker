using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDamage : MonoBehaviour
{
    public float Ob_MaxHp = 100;
    protected float ob_CurrentHp;
   
    public float Get_Ob_CurrentHp() { return ob_CurrentHp; }

    public void GetExplosionDamage(float damage)
    {
        ob_CurrentHp -= damage;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        float Ob_Damaged = collision.relativeVelocity.magnitude; //��ݷ�
        ob_CurrentHp -= (int)Ob_Damaged; //��ݷ���ŭ ü���� ����
    }
    private void Start()
    {
        ob_CurrentHp = Ob_MaxHp; // ü���� maxü������ ����
    }

}
