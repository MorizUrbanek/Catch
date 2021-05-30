using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class PlayerData : NetworkBehaviour
{
    //public int catcherId;

    public NetworkVariableInt catcherId = new NetworkVariableInt(0);
    public string username;
    public TextManager textManager;

    public void UpdateUsername(string newName)
    {
        username = newName;
    }

    private void Start()
    {
        if (IsLocalPlayer)
        {
            textManager.ChangeText("Player " + catcherId.Value + 1);
        }
    }
}
