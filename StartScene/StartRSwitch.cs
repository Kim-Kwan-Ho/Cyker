using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRSwitch : MonoBehaviour // l => ���η�����(������)
{
    [Header("Laser")]
    public Transform l_Target;
    private LineRenderer laser;


    private Collider coll; //Ray Target�� Collider �ӽ� �����
    private bool isClicked = false; //�� ��ȯ Ŭ������
    // Start is called before the first frame update
    void Start()
    {
        laser = this.gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        laser.SetPosition(0, transform.position); 
        laser.SetPosition(1, l_Target.position); //������ ������ ���� ����


        if (RightHand_GetGrab() && transform.position.x >= 330 && isClicked == false && FadeEffect.isFading==false) 
        {
            
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
                if (hit.collider.CompareTag("EscapeBtn"))
                {
                    coll.enabled = true;
                    hit.collider.GetComponentInParent<ChapterButton>().EscapeBtn();
                }
                else if (hit.collider.CompareTag("ChapterBtn"))
                {
                    coll = hit.collider.GetComponent<Collider>();
                    coll.enabled = false;
                    hit.collider.GetComponent<AudioSource>().Play();
                    hit.collider.GetComponentInParent<ChapterButton>().ClickCptBtn();

                }
                else if (hit.collider.CompareTag("Button"))
                {
                    hit.collider.GetComponent<UiBtn>().ClickBtn();
                    if(hit.collider.GetComponent<UiBtn>().btnType == BtnType.Level && hit.collider.GetComponent<UiBtn>().isLocked == false)
                    {
                        laser.enabled = false;
                        isClicked = true;
                    }
                }
                else if (hit.collider.CompareTag("Run"))
                {
                    hit.collider.GetComponent<AudioSource>().Play();
                    Application.OpenURL("https://hakgwa.pcu.ac.kr/game");
                }
                else if (hit.collider.CompareTag("HowTo"))
                {
                    laser.enabled = false;
                    isClicked = true;
                    hit.collider.GetComponent<AudioSource>().Play();
                    StartCoroutine(FadeEffect.SceneFade("Tutorial"));
                }
            }
        }
    }

    public bool RightHand_GetGrab() // ���� Ʈ���Ű� ����
    {
        return OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
    }

}
