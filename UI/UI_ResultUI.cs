using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ResultUI : MonoBehaviour // ui => UI
{
    public GameMgr gameMgr;

    [Header("ResultUI Info")]
    public float ui_pauseTime = 2f; //������ ������ �����ִ� �ð�
    public float ui_TransparencyTime = 1; // n�ʵڿ� UI���� 1
    public float ui_Transparency = 0.1f; // ������ n��ŭ ������ �ö�
    public float ui_ComeTime = 0.8f; // n�ʵ��� UI�� ������ �ö��

    private Vector3 offset; // ������ġ���� �ش� ���͸�ŭ �̵�
    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        offset = new Vector3(0,5.5f,-10);
    }

    private void Update()
    {
        if (gameMgr.g_State == GameState.clear || gameMgr.g_State == GameState.fail) //������ Ŭ����ǰų� �������� ��
        {
            if (PlayerCtrl.p_State == PlayerState.normal) //�÷��̾ �븻���·� ���ƿ���
            {
                ui_pauseTime -= Time.deltaTime;
                
                if (ui_pauseTime < 0) //����ð��� ������
                {
                    if (!transform.GetChild(0).gameObject.activeSelf) // UI�� ������ ���� ���
                    {
                        // UI�� �Ѱ� alpha���� 0���� 1�� ������ �ø���.
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
                        this.gameObject.transform.position = Vector3.SmoothDamp(transform.position, transform.position + offset, ref vel, ui_ComeTime *0.1f); //��� UI�� ������ Ÿ�� ���������� �ű��.
                    else
                        transform.position = transform.position + offset;
                }
            }
        }
    }

    IEnumerator UI_ResultUITransparence(Image[] images, Text[] texts) // UI�� õõ�� �����ϰ� ����
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
                if (images[0].color.a >=1) // ������ 1�� �Ѿ�� break
                    break;
            }
        }
    }
}
