using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using System.Text;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.Connection;

public class Timer : NetworkBehaviour
{
    static int catcherId = 0;

    public float[] timePerPlayer = new float[6];
    bool gameRunning = false, gameOver = false;

    public int playerCount;
    private float roundTime = 0.1f;
    private float timePassed = 0f;

    public GameObject startButton;
    PrintManager printManager;

    static string[] players = new string[6];

    public void StartGame()
    {
        gameRunning = true;
        startButton.SetActive(false);

        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            int id = client.PlayerObject.gameObject.GetComponentInChildren<PlayerData>().catcherId.Value;
            string name = client.PlayerObject.gameObject.GetComponentInChildren<PlayerData>().username;

            SetPlayers(name, id);

        }
    }

    public static void SetCatcherId(int id)
    {
        catcherId = id;
    }

    public static void SetPlayers(string name, int id)
    {
        Debug.Log(name);
        players[id] = name;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCount > 0)
        {
            printManager = GameObject.Find("List").GetComponent<PrintManager>();
        }

        if (playerCount > 1)
        {
            if (IsHost)
            {
                startButton.SetActive(true);
            }
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
            //SortPlayerScore();
            PrintPlayerScore();
            StartNewRound();
        }
    }

    private void PrintPlayerScore()
    {
        for (int i = 0; i < timePerPlayer.Length; i++)
        {
            playerRanking.Append($"Player {i + 1}: {timePerPlayer[i].ToString("0.00")} \n");
        }
        printManager.PrintTextClientRpc(playerRanking.ToString());
    }

    private void SortPlayerScore()
    {
        Array.Sort(timePerPlayer);
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
