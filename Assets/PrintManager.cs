using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using TMPro;

public class PrintManager : NetworkBehaviour
{

    public TextMeshProUGUI text;

    [ClientRpc]
    public void PrintTextClientRpc(string notification)
    {
        text.text = notification;
    }
}
