using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpread : UnitWeapon
{

    public GameObject w_SpreadHitObj; // �浹�� ��Ÿ���� ����Ʈ
    // Start is called before the first frame update

    protected override void Update()
    {
        base.Update();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Weapon")) // �� �������� ���� ����ϱ� ������ ����
            return;


        if (collision.collider.CompareTag("Enemy")) // ���� �浹�� �浹 �������� ������ ����Ʈ�� ������ �������� �ְ� ����
        {
            Instantiate(w_SpreadHitObj, transform.position, Quaternion.identity);

            collision.collider.GetComponent<EnemyMain>().TakeDamage(w_Damage);
            Destroy(gameObject);

        }
        else // ���� �ƴҰ�� �浹 ����Ʈ �߻� �� ����
        {
            Instantiate(w_SpreadHitObj, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

}
