using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrack : UnitWeapon // 기존 스킬처리를 여기서 작성했지만 회의를 통해 스킬을 사용하는 곳에서 계산하기로함 (유닛)
                                      // 유닛 스킬 발사위치 범위지정이기 때문에 해당 오브젝트는 이펙트만 발생함 (계산은 UnitTrack에서 작동함)
{

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
    }


}
