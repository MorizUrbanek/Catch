using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Text;

public class CatchPlayerCheck : NetworkBehaviour
{
    public CapsuleCollider playerCollider;
    public float sphereRadius;
    public LayerMask layerMask;
    private Vector3 origin;

    //private Timer timer = Timer.GetInstance();
    private CatchPlayer catchPlayer;
   // private Timer timerInstance;
    

    private void Start()
    {
        //text = GameObject.Find("Timelist").GetComponent<TextManager>();

        if (IsHost && IsOwner)
        {
            SelectFirstCatcherServerRpc();
        }

        if (IsLocalPlayer)
        {
            catchPlayer = gameObject.GetComponent<CatchPlayer>();
            int id = gameObject.GetComponent<PlayerData>().catcherId.Value;
            //string username = gameObject.GetComponent<PlayerData>().username;
            
            //Timer.SetPlayers(username, id);
            // timerInstance = GameObject.Find("Timer").GetComponent<Timer>();
        }
    }

    [ServerRpc]
    private void SelectFirstCatcherServerRpc()
    {
        gameObject.GetComponent<CatchPlayer>().Catched();
    }

    void Update()
    {

        origin = transform.position;

        if (IsLocalPlayer && IsOwner)
        {
            if (Input.GetButton("Catch") && catchPlayer.GetIsAttacker())
            {
                Debug.Log("catched");
                CatchPlayerServerRpc();
            }
        }
    }

    [ServerRpc]
    private void CatchPlayerServerRpc()
    {
        Collider[] hitColliders = Physics.OverlapSphere(origin, sphereRadius, layerMask);

        foreach (Collider catched in hitColliders)
        {
            if (catched.transform != transform)
            {
                if (CatchCeck(catched)) { return; }
            }
        }
    }

    private bool CatchCeck(Collider catched)
    {
        var catchedPlayer = catched.transform.GetComponent<CatchPlayer>();
        if (catchedPlayer != null)
        {
            int id = catched.transform.GetComponent<PlayerData>().catcherId.Value;
            //string username = catched.transform.GetComponent<PlayerData>().username.Value;
            Timer.SetCatcherId(id);
            //Timer.SetPlayers(username, id);
            catchedPlayer.Catched();
            gameObject.GetComponent<CatchPlayer>().Released();
            return true;
        }
        return false;
    }
}
