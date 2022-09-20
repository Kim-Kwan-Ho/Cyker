using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundVibOnOff : MonoBehaviour
{
    public static GameObject soundVib; // �ش� ��ũ��Ʈ �̱��� �������� ����ƽó��
    public static bool soundOn = true; // ���� OnOff ����
    public static bool vibOn = true; // ���� OnOff ����
    private void Awake()
    {
        if (soundVib == null) // �̱��� ����
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

    public static void TurnVib() // ���� OnOff
    {
        vibOn = !vibOn;
    }



    public static void TurnSound() // �Ҹ� OnOff
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
