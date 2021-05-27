using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;

public class Timer : NetworkBehaviour
{
    static int catcherId = 0;

    public float[] timePerPlayer = new float[6];
    bool gameStarted = true, gameOver = false;

    private float roundTime = 3f;
    private float timePassed = 0f;


    public static void SetCatcherId(int id)
    {
        catcherId = id;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) { return; }

        if (gameStarted)
        {
            timePerPlayer[catcherId] += Time.deltaTime;
            UpdateGlobalTime();
        }
        else if (gameOver)
        {
            EndGame();
        }
    }

    private void UpdateGlobalTime()
    {
        timePassed += Time.deltaTime;
        if (timePassed / 60 >= roundTime)
        {
            StartNewRound();
        }
    }

    private void StartNewRound()
    {
        Debug.Log("New Round Starting...");
    }

    private void EndGame()
    {
        throw new NotImplementedException();
    }
}
