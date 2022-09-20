using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDamage_Exploder : GetDamage
{
    void Update()
    {
        if (ob_CurrentHp <= 0)
        {
            if (this.gameObject.CompareTag("Unit"))
            {
                gameObject.GetComponent<UnitBoom>().UseSkill();
            }
            gameObject.GetComponent<Explosion>().Set_Explode_Dur(true);
        }
    }
}
