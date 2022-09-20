using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitFireGuide : MonoBehaviour
{
    public GameObject[] enableObjs;
    void Update()
    {
        if (PlayerCtrl.p_State == PlayerState.normal)
        {
            for(int i = 0; i < enableObjs.Length; i++)
            {
                enableObjs[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < enableObjs.Length; i++)
            {
                enableObjs[i].SetActive(false);
            }
        }
    }
}
