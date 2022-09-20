using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpread : UnitMain
{
    public GameObject w_SpreadGob;
    
    public float bulletSpeed = 30;
    public float angle = 30;
    public AudioClip audioClip;

    private GameObject[] w_Spreads = new GameObject[3];
    private Vector3 w_SpreadVelocity = Vector3.zero;
    private bool isSaveVelocity = false;
    
    private void FixedUpdate()
    {
        if (PlayerCtrl.p_State == PlayerState.fire && !isSaveVelocity) // ������ �߻�� ���� �ӵ��� ����
        {
            w_SpreadVelocity = gameObject.GetComponent<Rigidbody>().velocity;
            w_SpreadVelocity.y = 0;
            isSaveVelocity = true;
        }
    }



    public override void UseSkill()
    {
        base.UseSkill();
        GetComponent<TrailRenderer>().enabled = false;
        this.GetComponent<Collider>().enabled = false;
        GetComponent<AudioSource>().clip = audioClip;
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().volume = 1;
        GetComponent<AudioSource>().enabled = true;
        int w_SpreadsNum = w_Spreads.Length;
        for (int i = 0; i < w_SpreadsNum; i++)
        {
            float unit_radius = gameObject.GetComponent<SphereCollider>().radius + 0.5f;
            Vector3 bulletPos = w_SpreadVelocity.normalized * unit_radius; //������ ��ġ
            Quaternion bulletAngle = Quaternion.AngleAxis(angle * 2 / (w_SpreadsNum - 1) * i - angle, Vector3.up); //up���� �������� ȸ�� angle��ŭ ����

            w_Spreads[i] = Instantiate(w_SpreadGob, transform.position + bulletAngle * bulletPos, Quaternion.LookRotation(bulletAngle * w_SpreadVelocity.normalized)); //���ư��� ������ �ٶ󺸸� ����
            w_Spreads[i].GetComponent<Rigidbody>().velocity = bulletAngle * w_SpreadVelocity.normalized * bulletSpeed;
        }
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}