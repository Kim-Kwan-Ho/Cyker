using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWeapon : MonoBehaviour // w => ����
{
    public float w_LifeTime = 3; // ���� ���ӽð�
    public float w_Damage = 50; // ���� ������

    // Update is called once per frame
    protected virtual void Update() // ���� ���ӽð��� ������ �ı���
    {
        w_LifeTime -= Time.deltaTime; 
        if (w_LifeTime < 0)
            Destroy(gameObject);
    }

    public virtual void OnCollisionEnter(Collision collision) // �浹�� ������ ������ ��ŭ ���ظ� ����
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<EnemyMain>().TakeDamage(w_Damage);
            Destroy(gameObject);
        }
    }
}
