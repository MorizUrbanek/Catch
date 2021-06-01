using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameInputController : MonoBehaviour
{

    public void OnSelect()
    {
       //Disable Movement
    }

    public void OnDeselect()
    {
        //enable Movement
        gameObject.SetActive(false);
    }
}
