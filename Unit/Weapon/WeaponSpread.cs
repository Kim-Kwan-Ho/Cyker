using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpread : UnitWeapon
{

    public GameObject w_SpreadHitObj; // 충돌시 나타나는 이펙트
    // Start is called before the first frame update

    protected override void Update()
    {
        base.Update();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Weapon")) // 한 지점에서 같이 출발하기 때문에 무시
            return;


        if (collision.collider.CompareTag("Enemy")) // 적과 충돌시 충돌 지점에서 터지는 이펙트와 적에게 데미지를 주고 삭제
        {
            Instantiate(w_SpreadHitObj, transform.position, Quaternion.identity);

            collision.collider.GetComponent<EnemyMain>().TakeDamage(w_Damage);
            Destroy(gameObject);

        }
        else // 적이 아닐경우 충돌 이펙트 발생 및 삭제
        {
            Instantiate(w_SpreadHitObj, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

}
