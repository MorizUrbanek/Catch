using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    private Cinemachine3rdPersonFollow cameraFollow;


    public void Start()
    {
        if (IsLocalPlayer)
        {
            cameraFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            virtualCamera.gameObject.SetActive(true);

            enabled = true;
        }
    }

}
