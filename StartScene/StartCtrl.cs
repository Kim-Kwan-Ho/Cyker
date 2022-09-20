using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartCtrl : MonoBehaviour  // p => �÷��̾� , pRH => �÷��̾� ���� ����ġ , l => ���η�����(������)
{
    [Header("Player Target Position")]
    public Vector3 p_TargetVec; // �÷��̾� ��ǥ ��ġ


    [Header("Before Enter Objs")]
    public GameObject[] dis_Objs; // ���� �� ��Ȱ��ȭ ������Ʈ
    public Text start_Txt; // �ȳ�����
    private float text_A;
    private bool text_Up;


    [Header("After Enter Objs")]
    public GameObject[] ena_Objs; // ���� �� Ȱ��ȭ ������Ʈ
    public Image[] ena_ImgsAlpha;
    public Text[] ena_TxtAlpha;


    [Header("Check Click & Entered")]
    private bool isClicked; // Ŭ�� ����
    private bool isEntered; // ���� ����
    private float img_A;


    [Header("Laser")]
    private LineRenderer pRH_Laser;
    private bool l_Check; // ������ Ȱ��ȭ ����



    void Start()
    {
        ResetState(); 

        StartCoroutine("TextAlphaChange");
        
        pRH_Laser = GetComponentInChildren<LineRenderer>();
    }

    void Update()
    {
        start_Txt.color = new Color(start_Txt.color.r, start_Txt.color.g, start_Txt.color.b, text_A);


        if (isClicked==false && OVRInput.GetDown(OVRInput.Button.Any) && !OVRInput.GetDown(OVRInput.Button.Right)) // ��Ʈ���� Ŭ�� Ȯ��
        {
            isClicked=true;
        }


        if (isClicked) // Ŭ���� �÷��̾� ��ǥ�������� �̵� �� ���� Fadeȿ�� ����
        {
            transform.position = Vector3.Lerp(transform.position, p_TargetVec, 1 * Time.deltaTime);

            for(int i=0; i<ena_ImgsAlpha.Length; i++)
            {
                ena_ImgsAlpha[i].color = new Color(1,1,1, img_A);
            }
            for(int i=0; i<ena_TxtAlpha.Length; i++)
            {
                ena_TxtAlpha[i].color = new Color(1,1,1, img_A);
            }
            if (isEntered == false)
            {
                StartCoroutine("EnterScene");
            }

        }
        if (isEntered && !pRH_Laser.enabled && l_Check == false) // ���� ��Ʈ�ѷ� ������ Ȱ��ȭ
        {
            pRH_Laser.enabled = true;
            l_Check = true;
        }

    }
    private void ResetState() //���۽� ���� �� ������Ʈ �ʱ�ȭ
    {
        l_Check = false;
        text_Up = false;
        isEntered = false;
        isClicked = false;
        text_A = 1.0f;
        img_A = 0;
        for (int i = 0; i < dis_Objs.Length; i++)
        {
            dis_Objs[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < ena_Objs.Length; i++)
        {
            ena_Objs[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator EnterScene() // �� ����
    {
        yield return new WaitForSeconds(1.3f);
        isEntered = true;
        for (int i=0; i < dis_Objs.Length; i++)
        {
            dis_Objs[i].gameObject.SetActive(false);
        }
        for(int i=0; i<ena_Objs.Length; i++)
        {
            ena_Objs[i].gameObject.SetActive(true);
        }
        StartCoroutine("ImageAlphaChange");
    }

    private IEnumerator ImageAlphaChange() // ���Խ� Fadeȿ��
    {
        while (true)
        {
            if (img_A <= 1)
            {
                img_A += Time.deltaTime * 0.5f;
            }
            else
            {
                img_A = 1;
                break;
            }
            yield return null;
        }
    }
    private IEnumerator TextAlphaChange() // Text Fadeȿ��
    {
        while (true)
        {
            if (text_Up)
            {
                text_A += Time.deltaTime * 0.7f;
                if (text_A >= 1)
                {
                    text_Up=false;
                }
            }
            else
            {
                text_A -= Time.deltaTime * 0.7f;
                if (text_A <= 0)
                {
                    text_Up = true;
                }
            }
            yield return null;
        }
    }
}
