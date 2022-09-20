using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParabola : MonoBehaviour
{
    public void DrawParabolaLine(Vector3 startPos, Vector3 velocity, LineRenderer parabloaLine, float parabolaTime = 1f) // �������� �׸�(������, �ӵ�, ���η�����, n�ʸ�ŭ ������ ǥ��)
    {
        parabloaLine.positionCount = (int)(50 * parabolaTime);
        int frame = parabloaLine.positionCount;
        Vector3 objPosition = startPos;
        parabloaLine.SetPosition(0, objPosition); // ������ ������
        for (int i = 1; i < frame; i++) // �߷°��ӵ���ŭ ���ӵ��� �����ָ� ���� �ϳ��� ����
        {
            velocity += Physics.gravity * 0.02f; 
            objPosition += velocity * 0.02f; 
            parabloaLine.SetPosition(i, objPosition);
        }
    }
}