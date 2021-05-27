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
    public bool isCatcher;


    public void Test()
    {
        Debug.Log("Meine ID: " + catcherId.Value);
    }
}
