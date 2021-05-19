using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wall Running")]
    public float wallDistance = .6f;
    public float minimumJumpHeight = 1.5f;
    public float wallRunGravity = 5;
    public float wallJumpForce = 6;
    public float wallFrontJumpForce = 2.5f;

    bool isWallLeft;
    bool isWallRight;
    bool isWallFront;
    bool isWallrunning = false;
    bool climbingLedge = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    RaycastHit frontWallHit;

    Rigidbody rb;
    public LayerMask walllayer;
    public Transform ledgeUpCheck;
    public Transform ledgeLevelCheck;
    public Transform direction;

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

        if (CheckForClimableLedge() && !climbingLedge)
        {
            if (Physics.Raycast(ledgeLevelCheck.position, Vector3.down, wallDistance * 5, walllayer))
            {
                Debug.Log("Start Climbing");
                climbingLedge = true;
                rb.velocity = new Vector3(0, rb.velocity.y / 8, 0);

            }

        }

        if (climbingLedge)
        {
            if (isWallFront)
            {
                rb.AddForce(direction.up * 6 + direction.forward * 2f, ForceMode.Force);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 20, rb.velocity.z);
                climbingLedge = false;
            }
        }
    }

    void CheckWall()
    {
        isWallLeft = Physics.BoxCast(transform.position, new Vector3(0.1f, 2, 0.1f), -direction.right, out leftWallHit, transform.rotation, wallDistance, walllayer);
        isWallRight = Physics.BoxCast(transform.position, new Vector3(0.1f, 2, 0.1f), direction.right, out rightWallHit, transform.rotation, wallDistance, walllayer);
        isWallFront = Physics.BoxCast(transform.position, new Vector3(0.1f, 2, 0.1f), direction.forward, out frontWallHit, transform.rotation, wallDistance, walllayer);  
    }

    private void OnDrawGizmos()
    {
        if (isWallLeft)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(direction.position + -direction.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(direction.position + -direction.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
        }
        
        if (isWallRight)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(direction.position + direction.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(direction.position + direction.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
        } 
        
        if (isWallFront)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(direction.position + direction.forward * wallDistance, new Vector3(0.1f, 2, 0.1f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(direction.position + direction.forward * wallDistance, new Vector3(0.1f, 2, 0.1f));
        }
        
    }

    //private void OnDrawGizmos()
    //{
    //    if (isWallLeft)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireCube(body.position + -body.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
    //    }
    //    else
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(body.position + -body.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
    //    }

    //    if (isWallRight)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireCube(body.position + body.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
    //    }
    //    else
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(body.position + body.right * wallDistance, new Vector3(0.1f, 2, 0.1f));
    //    }

    //    if (isWallFront)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawWireCube(body.position + body.forward * wallDistance, new Vector3(0.1f, 2, 0.1f));
    //    }
    //    else
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(body.position + body.forward * wallDistance, new Vector3(0.1f, 2, 0.1f));
    //    }

    //}

    bool CanWallRun()
    {
        return !Physics.Raycast(direction.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun()
    {
        if (!isWallrunning && isWallFront && rb.velocity.y > 1)
        {
            rb.AddForce((direction.up + direction.forward / 5) * wallFrontJumpForce * 100, ForceMode.Force);
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
                wallRunJumpDirection = direction.up / 2 + leftWallHit.normal;
            }
            else if (isWallRight)
            {
                wallRunJumpDirection = direction.up / 2 + rightWallHit.normal;
            }
            else if (isWallFront)
            {
                wallRunJumpDirection = direction.up / 2 + frontWallHit.normal;
            }
            rb.AddForce(wallRunJumpDirection * wallJumpForce * 100, ForceMode.Force);
        }

    }

    void StopWallRun()
    {
        isWallrunning = false;
        rb.useGravity = true;
    }

    bool CheckForClimableLedge()
    {
        bool ledgeCheckUp = Physics.Raycast(ledgeUpCheck.position, direction.forward, wallDistance, walllayer);

        if (isWallFront && !ledgeCheckUp)
        {
            return true;
        }
        return false;
    }
}
