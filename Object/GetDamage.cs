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
        float Ob_Damaged = collision.relativeVelocity.magnitude; //충격량
        ob_CurrentHp -= (int)Ob_Damaged; //충격량만큼 체력을 깎음
    }
    private void Start()
    {
        ob_CurrentHp = Ob_MaxHp; // 체력을 max체력으로 설정
    }

}
