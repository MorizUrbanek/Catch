using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Timer : NetworkBehaviour
{
    public float[] timePerPlayer = new float[6];
    bool gameStarted = true;
    static int catcherId = 0;

    public static void SetCatcherId(int id)
    {
        catcherId = id;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer) { return; }

        if (gameStarted)
        {
            timePerPlayer[catcherId] += Time.deltaTime;
        }
    }
}
