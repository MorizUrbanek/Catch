using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Spawning;
using MLAPI.Transports.PhotonRealtime;

public class ConnectionManager : MonoBehaviour
{
    public GameObject connectionButtonPanel;
    private int playerCount;
    PlayerData player;
    string username;

    public void Host()
    {
        connectionButtonPanel.SetActive(false);
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost(GetRandomSpawn(), Quaternion.identity);
        InitPlayerData(NetworkManager.Singleton.LocalClientId);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == "1234";
        callback(true, null, approve, GetRandomSpawn(), Quaternion.identity);
        if (approve)
        {
            InitPlayerData(clientId);
        }
    }

    private void InitPlayerData(ulong clientId)
    {
        playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponentInChildren<PlayerData>();

        if (player != null)
        {
            player.catcherId.Value = playerCount - 1;
            player.username = username;
            //player.Test();
        }
    }

    private Vector3 GetRandomSpawn()
    {
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);

        return new Vector3(x, 1f, z);
    }

    public void UpdateUsername(string newName)
    {
        username = newName;
        Debug.Log(username);
    }

    public void Client()
    {
        connectionButtonPanel.SetActive(false);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("1234");
        NetworkManager.Singleton.StartClient();
    }
}
