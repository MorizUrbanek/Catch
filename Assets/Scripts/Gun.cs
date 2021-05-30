using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MLAPI;


public class Gun : NetworkBehaviour
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
    private float fireCooldown = .5f;
    private float fireCooldownend = 0;

    private CatchPlayer catchPlayer;

    private void Start()
    {
        if (IsLocalPlayer)
        {
            catchPlayer = gameObject.GetComponentInParent<CatchPlayer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetButtonDown("Fire1") || Input.GetAxis("Fire1") > 0) && isAiming && Time.time > fireCooldownend)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        fireCooldownend =Time.time + fireCooldown;
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
        if (IsLocalPlayer)
        {
            if(catchPlayer.isActuallyAttacker == false)
            {
                isAiming = false;
            }
            else
            {
                isAiming = value;
            }
            mainCam.SetActive(!isAiming);
            aimCam.SetActive(isAiming);
        }
    }
}
