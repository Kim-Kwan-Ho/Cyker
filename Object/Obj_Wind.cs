using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj_Wind : MonoBehaviour // ob => ������Ʈ
{
    public enum WindModes
    {
        windForce,
        windImpulse,
        windBoost
    }
    public Mesh arrowMesh; // ȭ��ǥ mesh

    [Header("Wind Information")]
    public WindModes windMode = WindModes.windForce;

    [Range(0,100)]
    public float ob_WindPower = 0; //�ٶ� ����
    
    public bool ob_WindSizeScaling = false;
    public float ob_WindSize = 1; //�ٶ� ���� ũ��

    private Vector3 ob_WindDirection; // �ٶ� ����

    private void Update() // �ٶ� ���� ������Ʈ
    {
        Vector3 Ob_WindDirection = transform.GetChild(0).gameObject.transform.position;
        ob_WindDirection = (Ob_WindDirection - gameObject.transform.position).normalized;
    }

    private void OnDrawGizmos() // �ٶ� ���� �� �ٶ� ���� ǥ��
    {
        if(ob_WindSizeScaling) transform.localScale = Vector3.one * ob_WindSize; // ������ ���� ����
        Gizmos.color = Color.yellow;
        Gizmos.DrawMesh(arrowMesh, transform.position + Vector3.up, transform.rotation, Vector3.one);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Weapon"))
            return;

        if (other.GetComponent<Rigidbody>())
        {
            switch (windMode)
            {
                case WindModes.windForce:
                    other.GetComponent<Rigidbody>().AddForce(ob_WindDirection * ob_WindPower, ForceMode.Force);
                    break;
                case WindModes.windImpulse:
                    other.GetComponent<Rigidbody>().AddForce(ob_WindDirection * ob_WindPower, ForceMode.Impulse);
                    break;
                case WindModes.windBoost:
                    other.GetComponent<Rigidbody>().velocity = ob_WindDirection * ob_WindPower;
                    break;
                default:
                    break;
            }
        }
    }
}

