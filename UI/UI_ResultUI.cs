using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultUI : MonoBehaviour // ui => UI
{
    public GameMgr gameMgr;

    [Header("ResultUI Info")]
    public float ui_pauseTime = 2f; //게임이 끝나고 멈춰있는 시간
    public float ui_TransparencyTime = 1; // n초뒤에 UI투명도 1
    public float ui_Transparency = 0.1f; // 투명도가 n만큼 서서히 올라감
    public float ui_ComeTime = 0.8f; // n초동안 UI가 서서히 올라옴

    private Vector3 offset; // 현재위치에서 해당 벡터만큼 이동
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        offset = new Vector3(0,5.5f,-10);
    }

    private void Update()
    {
        if (gameMgr.g_State == GameState.clear || gameMgr.g_State == GameState.fail) //게임이 클리어되거나 실패했을 때
        {
            if (PlayerCtrl.p_State == PlayerState.normal) //플레이어가 노말상태로 돌아오면
            {
                ui_pauseTime -= Time.deltaTime;
                
                if (ui_pauseTime < 0) //퍼즈시간이 끝나면
                {
                    if (!transform.GetChild(0).gameObject.activeSelf) // UI가 켜지지 않은 경우
                    {
                        // UI를 켜고 alpha값을 0에서 1로 서서히 올린다.
                        transform.GetChild(0).gameObject.SetActive(true);
                        Image[] images = gameObject.GetComponentsInChildren<Image>();
                        Text[] texts = gameObject.GetComponentsInChildren<Text>();
                        foreach (Image image in images)
                        {
                            Color color = image.color;
                            color.a = 0;
                            image.color = color;
                        }
                        foreach (Text text in texts)
                        {
                            Color color = text.color;
                            color.a = 0;
                            text.color = color;
                        }
                        StartCoroutine(UI_ResultUITransparence(images, texts));
                    }
                    Vector3 vel = Vector3.zero;
                    if (Vector3.Distance(transform.position, transform.position + offset) > 0.02f)
                        this.gameObject.transform.position = Vector3.SmoothDamp(transform.position, transform.position + offset, ref vel, ui_ComeTime *0.1f); //결과 UI를 서서히 타겟 포지션으로 옮긴다.
                    else
                        transform.position = transform.position + offset;
                }
            }
        }
    }

    IEnumerator UI_ResultUITransparence(Image[] images, Text[] texts) // UI를 천천히 선명하게 만듬
    {
        if (ui_pauseTime < 0)
        {
            while(true)
            {
                yield return new WaitForSeconds(ui_TransparencyTime * ui_Transparency);
                foreach (Image image in images)
                {
                    Color color = image.color;
                    color.a += ui_Transparency;
                    image.color = color;
                }
                foreach (Text text in texts)
                {
                    Color color = text.color;
                    color.a += ui_Transparency;
                    text.color = color;
                }
                if (images[0].color.a >=1) // 투명도가 1이 넘어가면 break
                    break;
            }
        }
    }
}
