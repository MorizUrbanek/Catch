using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using System.Text;
using MLAPI.Messaging;

public class Timer : NetworkBehaviour
{
    static int catcherId = 0;

    public float[] timePerPlayer = new float[6];
    bool gameRunning = false, gameOver = false;


    public void StartGame()
    {
        gameRunning = true;
    }


    public int playerCount;
    private float roundTime = 0.5f;
    private float timePassed = 0f;

    PrintManager printManager;

    public static void SetCatcherId(int id)
    {
        catcherId = id;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerCount > 0)
        {
            printManager = GameObject.Find("List").GetComponent<PrintManager>();
        }

        if (!IsServer) { return; }
        playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (gameRunning)
        {
            timePerPlayer[catcherId] += Time.deltaTime;
            UpdateGlobalTime();
        }
        else if (gameOver)
        {
            EndGame();
        }
    }


    StringBuilder playerRanking = new StringBuilder();
    private void UpdateGlobalTime()
    {
        timePassed += Time.deltaTime;
        if (timePassed / 60 >= roundTime)
        {
            gameRunning = false;
            SortPlayerScore();
            PrintPlayerScore();
            StartNewRound();
        }
    }

    private void PrintPlayerScore()
    {
        printManager.PrintTextClientRpc(playerRanking.ToString());
    }

    private void SortPlayerScore()
    {
        Array.Sort(timePerPlayer);

        for(int i = 6 - playerCount; i < timePerPlayer.Length; i++)
        {
            playerRanking.Append($"Time: {timePerPlayer[i].ToString("0.00")} \n");
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
