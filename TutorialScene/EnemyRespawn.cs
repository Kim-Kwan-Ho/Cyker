using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public GameObject e_Cur;
    public GameObject e_Obj;
    public GameMgr gameMgr;
    public Vector3 e_Pos;
    public Vector3 e_Rot;
    private bool isSpawn = false;

    void Update()
    {
        if (e_Cur == null &&isSpawn == false)
        {
            isSpawn = true;
            Invoke("SpawnEnemy", 2);
        }
    }

    void SpawnEnemy()
    {
        e_Cur = Instantiate(e_Obj, e_Pos, Quaternion.Euler(e_Rot));
        gameMgr.e_Count++;
        isSpawn = false;
    }
}
