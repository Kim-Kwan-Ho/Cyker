using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeapon : MonoBehaviour // w => 무기
{
    public float w_LifeTime = 3; // 무기 지속시간
    public float w_Damage = 50; // 무기 데미지

    // Update is called once per frame
    protected virtual void Update() // 무기 지속시간이 끝나면 파괴됨
    {
        w_LifeTime -= Time.deltaTime; 
        if (w_LifeTime < 0)
            Destroy(gameObject);
    }

    public virtual void OnCollisionEnter(Collision collision) // 충돌시 적에게 데미지 만큼 피해를 입힘
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<EnemyMain>().TakeDamage(w_Damage);
            Destroy(gameObject);
        }
    }
}
