using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBoom : UnitMain
{
    public override void UseSkill()
    {
        base.UseSkill();
        if (SoundVibOnOff.vibOn == true && u_IsVibe == false)
        {
            StartCoroutine(VibrationAfterSkillUse());
            u_IsVibe = true;
        }
        this.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Explosion>().Set_Explode_Dur(true);
    }


}
