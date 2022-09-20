using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDestroyer : MonoBehaviour // 이펙트 삭제용 (단순처리)
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
