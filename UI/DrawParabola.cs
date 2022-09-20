using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParabola : MonoBehaviour
{
    public void DrawParabolaLine(Vector3 startPos, Vector3 velocity, LineRenderer parabloaLine, float parabolaTime = 1f) // 포물선을 그림(시작점, 속도, 라인렌더러, n초만큼 포물선 표시)
    {
        parabloaLine.positionCount = (int)(50 * parabolaTime);
        int frame = parabloaLine.positionCount;
        Vector3 objPosition = startPos;
        parabloaLine.SetPosition(0, objPosition); // 포물선 시작점
        for (int i = 1; i < frame; i++) // 중력가속도만큼 가속도를 더해주며 점을 하나씩 찍음
        {
            velocity += Physics.gravity * 0.02f; 
            objPosition += velocity * 0.02f; 
            parabloaLine.SetPosition(i, objPosition);
        }
    }
}