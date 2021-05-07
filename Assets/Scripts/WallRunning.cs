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
    public float wallFrontJumpForce = 2.5f;

    bool isWallLeft;
    bool isWallRight;
    bool isWallFront;
    bool isWallrunning = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    RaycastHit frontWallHit;

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
            if (isWallLeft || isWallRight || isWallFront)
            {
                StartWallRun();
            }
            else
            {
                if (isWallrunning)
                {
                    StopWallRun();
                }
            }
        }
        else
        {
            if (isWallrunning)
            {
                StopWallRun();
            }
        }
    }

    void CheckWall()
    {
        isWallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallDistance, walllayer);
        isWallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallDistance, walllayer);
        isWallFront = Physics.Raycast(transform.position, transform.forward, out frontWallHit, wallDistance, walllayer);
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun()
    {
        if (!isWallrunning && isWallFront && rb.velocity.y > 1)
        {
            rb.AddForce((transform.up + transform.forward / 5) * wallFrontJumpForce * 100, ForceMode.Force);
        }

        if (!isWallrunning)
        {
            isWallrunning = true;
        }
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if (Input.GetButtonDown("Jump"))
        {
            Vector3 wallRunJumpDirection = Vector3.zero;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (isWallLeft)
            {
                wallRunJumpDirection = transform.up/2 + leftWallHit.normal;
                rb.AddForce(wallRunJumpDirection * wallJumpForce * 100, ForceMode.Force);
            }
            else if (isWallRight)
            {
                wallRunJumpDirection = transform.up/2 + rightWallHit.normal;
                rb.AddForce(wallRunJumpDirection * wallJumpForce * 100, ForceMode.Force);
            }
            else if (isWallFront)
            {
                wallRunJumpDirection = transform.up / 2 + frontWallHit.normal;
                rb.AddForce(wallRunJumpDirection * wallJumpForce * 100, ForceMode.Force);
            }
            
        }
    }

    void StopWallRun()
    {
        isWallrunning = false;
        rb.useGravity = true;
    }
}
