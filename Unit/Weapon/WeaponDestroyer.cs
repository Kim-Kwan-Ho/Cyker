using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDestroyer : MonoBehaviour // ����Ʈ ������ (�ܼ�ó��)
{
    public float LifeTime = 5f;

    void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
