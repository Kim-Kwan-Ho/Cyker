using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundVibOnOff : MonoBehaviour
{
    public static GameObject soundVib; // 해당 스크립트 싱글톤 적용위해 스태틱처리
    public static bool soundOn = true; // 사운드 OnOff 여부
    public static bool vibOn = true; // 진동 OnOff 여부
    private void Awake()
    {
        if (soundVib == null) // 싱글톤 적용
        {
            soundVib = this.gameObject;
            DontDestroyOnLoad(soundVib);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static void TurnVib() // 진동 OnOff
    {
        vibOn = !vibOn;
    }



    public static void TurnSound() // 소리 OnOff
    {
        soundOn = !soundOn;
        if (soundOn)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }
    }


}
