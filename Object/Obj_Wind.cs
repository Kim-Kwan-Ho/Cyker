using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj_Wind : MonoBehaviour // ob => 오브젝트
{
    public enum WindModes
    {
        windForce,
        windImpulse,
        windBoost
    }
    public Mesh arrowMesh; // 화살표 mesh

    [Header("Wind Information")]
    public WindModes windMode = WindModes.windForce;

    [Range(0,100)]
    public float ob_WindPower = 0; //바람 세기
    
    public bool ob_WindSizeScaling = false;
    public float ob_WindSize = 1; //바람 영역 크기

    private Vector3 ob_WindDirection; // 바람 방향

    private void Update() // 바람 방향 업데이트
    {
        Vector3 Ob_WindDirection = transform.GetChild(0).gameObject.transform.position;
        ob_WindDirection = (Ob_WindDirection - gameObject.transform.position).normalized;
    }

    private void OnDrawGizmos() // 바람 방향 및 바람 영역 표시
    {
        if(ob_WindSizeScaling) transform.localScale = Vector3.one * ob_WindSize; // 스케일 비율 조정
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

