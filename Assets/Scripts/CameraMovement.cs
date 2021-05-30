using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class CameraMovement : NetworkBehaviour
{
    public float rotationSpeed = 1;
    float joyStickSpeedVertical = 13;
    float joyStickSpeedHorizontal = 8;
    public Transform direction, body;
    public Gun gun;
    float mouseX, mouseY;

    public CatchPlayer catchPlayer;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        CamControl();
        if (IsLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }

    void CamControl()
    {
        mouseX += Input.GetAxisRaw("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxisRaw("Mouse Y") * rotationSpeed;

        mouseX += Input.GetAxisRaw("CameraVertical") * joyStickSpeedVertical;
        mouseY -= Input.GetAxisRaw("CameraHorizontal") * joyStickSpeedHorizontal;

        mouseY = Mathf.Clamp(mouseY, -55, 75);

        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        if (gun.isAiming)
        {
            if (catchPlayer.isActuallyAttacker && IsLocalPlayer)
            {
                body.rotation = Quaternion.Euler(0, mouseX, 0);
            }
        }
        direction.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}
