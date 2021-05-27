using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Timer : NetworkBehaviour
{
    public float[] timePerPlayer = new float[6];
    bool gameStarted = true;
    static int catcherId = 0;
    static Timer instance;

    public static Timer GetInstance()
    {
        if(instance == null)
        {
            return instance = new Timer();
        }
        else
        {
            return instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsServer) { return; }

        if (gameStarted)
        {
            timePerPlayer[0] += Time.deltaTime;
        }
    }
}
