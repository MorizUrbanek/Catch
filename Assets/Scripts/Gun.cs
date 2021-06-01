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
    bool isShootingRope = false;
    float RopeSpeed = 5f;
    public Vector3 pullPoint;

    private float range = 100f;
    private float fireCooldown = .5f;
    private float fireCooldownend = 0;

    private CatchPlayer catchPlayer;
    [SerializeField] private GameObject rope;
    LineRenderer lineR;
    GameObject ropeEffect;

    private void Start()
    {
        if (IsLocalPlayer)
        {
            catchPlayer = gameObject.GetComponentInParent<CatchPlayer>();
            SpawnRope();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if ((Input.GetButtonDown("Fire1") || Input.GetAxis("Fire1") > 0) && isAiming && Time.time > fireCooldownend)
            {
                Shoot();
            }

            if (isShootingRope)
            {
                lineR.SetPosition(1, pullPoint);
                isPulling = true;
                isShootingRope = false;
            }
            else if (isPulling)
            {
                lineR.SetPosition(0, transform.position);
            }
            else if (ropeEffect.activeSelf)
            {
                ropeEffect.SetActive(false);
            }
        }
    }

    void Shoot()
    {
        fireCooldownend = Time.time + fireCooldown;
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
                ropeEffect.SetActive(true);
                isShootingRope = true;
                //isPulling = true;
            }

        }
    }

    private void SpawnRope()
    {
        ropeEffect = Instantiate(rope, Vector3.zero, Quaternion.identity);

        lineR = ropeEffect.GetComponent<LineRenderer>();

        lineR.SetPosition(0, transform.position);
        lineR.SetPosition(1, transform.position);

        ropeEffect.SetActive(false);
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
