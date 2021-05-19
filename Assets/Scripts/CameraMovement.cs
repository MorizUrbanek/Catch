using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float rotationSpeed = 1;
    public Transform direction, body;
    public Gun gun;
    float mouseX, mouseY;
    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        CamControl();
    }

    void CamControl()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        if (gun.isAiming)
        {
            body.rotation = Quaternion.Euler(0, mouseX, 0);
        }

        direction.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}
