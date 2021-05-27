using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using System;

public class CatchPlayer : NetworkBehaviour
{

    public NetworkVariableBool isAttacker = new NetworkVariableBool(false);
    public bool isActuallyAttacker = false;
    public MeshRenderer playerColor;

    public bool GetIsAttacker()
    {
        return isActuallyAttacker;
    }

    void Update()
    {
        isActuallyAttacker = isAttacker.Value;

        if (isActuallyAttacker)
        {
            playerColor.material.SetColor("_Color", Color.red);
        }
        else
        {
            playerColor.material.SetColor("_Color", Color.white);
        }
    }

    public void Catched()
    {
        if(isActuallyAttacker == false)
        {
            isAttacker.Value = true;
        }
    }

    public void Released()
    {
        if (isActuallyAttacker)
        {
            isAttacker.Value = false;
        }
    }
}
