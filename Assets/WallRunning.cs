using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    public float wallDistance = .6f;
    public float minimumJumpHeight = 1.5f;
    public float wallRunGravity = 1;
    public float wallJumpForce = 6;

    bool isWallLeft;
    bool isWallRight;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    Rigidbody rb;
    public LayerMask walllayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckWall();

        if (CanWallRun())
        {
            if (isWallLeft || isWallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void CheckWall()
    {
        isWallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallDistance, walllayer);
        isWallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallDistance, walllayer);
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun()
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        Debug.Log("Is wallrunning");

        if (Input.GetButtonDown("Jump"))
        {
            if (isWallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up/2 + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallJumpForce * 100, ForceMode.Force);
            }
            else if (isWallRight)
            {
                Vector3 wallRunJumpDirection = transform.up/2 + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallJumpForce * 100, ForceMode.Force);
            }
        }
    }

    void StopWallRun()
    {
        Debug.Log("Stop wallrun");
        rb.useGravity = true;
    }
}
