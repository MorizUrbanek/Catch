using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class PlayerData : NetworkBehaviour
{
    public int catcherId;
    public bool isCatcher;

    public int GetCatcherId()
    {
        return catcherId;
    }

    public void SetIsCatcher(bool isCatcher)
    {
        this.isCatcher = isCatcher;
    }

    public bool GetIsCatcher()
    {
        return isCatcher;
    }
}
