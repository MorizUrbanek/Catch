using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Gun : MonoBehaviour
{
    public GameObject mainCam;
    public GameObject aimCam;
    public Camera rayCam;
    public Rigidbody rb;

    [HideInInspector]
    public bool isAiming = false;
    public bool isPulling = false;
    public Vector3 pullPoint;

    private float range = 100f;

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetAxis("Fire1") > 0) && isAiming)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        Ray ray = rayCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.transform.CompareTag("Player"))
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(hit.normal * 5, ForceMode.Impulse);
                }
            }
            else if (hit.transform.CompareTag("Wall"))
            {
                pullPoint = hit.point;
                isPulling = true;
            }

        }
    }

    public void SetIsAiming(bool value)
    {
        isAiming = value;
        mainCam.SetActive(!isAiming);
        aimCam.SetActive(isAiming);
    }
}
