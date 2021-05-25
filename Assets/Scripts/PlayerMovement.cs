using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Movement Variablen
    [Header("Movement")]
    Rigidbody rb;
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    public float airMultiplier = 0.4f;
    Vector3 moveDirection;
    float horizontalMovement;
    float verticalMovement;

    [Header("Slopes")]
    RaycastHit slopeHit;
    Vector3 slopeDirection;
    float playerheight = 2f;

    [Header("Sprinting")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Sliding")]
    float slideTime = .5f;
    bool isSliding = false;
    Vector3 slidingDirection;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public Transform headCheck;
    public LayerMask groundMask;
    bool isGrounded;
    bool isHeadOnwall;
    float groundDistance = 0.4F;

    [Header("Direction (Playermodle)")]
    public Transform body;
    public Transform direction;
    Vector3 currentDirection;
    Quaternion bodyRotation;

    [Header("Gun Pulling")]
    public Gun gun;

    #endregion

    #region Wallrun Variablen
    [Header("Wall Running")]
    public float wallDistance = .6f;
    public float minimumJumpHeight = 1.3f;
    public float wallRunGravity = 4.8f;
    public float wallJumpForce = 6;
    public float wallFrontJumpForce = 4f;

    public LayerMask walllayer;
    public Transform ledgeUpCheck;
    public Transform ledgeLevelCheck;
    public Transform wallCheck;

    bool isWallLeft;
    bool isWallRight;
    bool isWallFront;
    bool isWallBack;
    bool isWallrunning = false;
    bool climbingLedge = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    RaycastHit frontWallHit;
    RaycastHit backWallHit;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isHeadOnwall = Physics.CheckSphere(headCheck.position, groundDistance, groundMask);

        MyInput();
        ControlSpeed();
        ControlDrag();
        if (!isSliding)
        {
            WallRun();
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isSliding)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.E) && isGrounded && (Mathf.Abs(rb.velocity.x) > 1 || Mathf.Abs(rb.velocity.z) > 1) && !isSliding && !OnSlope() && !gun.isAiming)
        {
            StartSliding();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && !isSliding)
        {
            gun.SetIsAiming(!gun.isAiming);
            body.rotation = direction.rotation;
            //if (!gun.isAiming)
            //{
            //    gun.isPulling = false;
            //}
        }

        //if (Input.GetKey(KeyCode.Mouse1))
        //{
        //    if (!isSliding)
        //    {
        //        if (!gun.isAiming)
        //        {
        //            gun.SetIsAiming(true);
        //            body.rotation = direction.rotation;
        //        }
        //    }
        //}
        //else
        //{
        //    if (gun.isAiming)
        //    {
        //        gun.SetIsAiming(false);
        //    }
        //}

        slopeDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    #region Sliding
    void StartSliding()
    {
        isSliding = true;
        body.rotation = Quaternion.Euler(-80, 0, 0);
        slidingDirection = moveDirection;
        Invoke("EndSliding", slideTime);
    }

    void EndSliding()
    {
        isSliding = false;
        body.rotation = Quaternion.Euler(0, 0, 0);
    }
    #endregion

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = direction.forward * verticalMovement + direction.right * horizontalMovement;
    }

    #region Controls
    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }
    #endregion

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(body.up * jumpForce, ForceMode.Impulse);
    }

    #region WallRun
    void WallRun()
    {
        CheckWall();
        if (CanWallRun() && !gun.isPulling && !OnSlope())
        {
            if (isWallLeft || isWallRight || isWallFront || isWallBack)
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

        if (CheckForClimableLedge() && !climbingLedge && isWallrunning && !isGrounded)
        {
            if (Physics.Raycast(ledgeLevelCheck.position, Vector3.down, wallDistance * 5, walllayer))
            {
                climbingLedge = true;
                rb.velocity = new Vector3(0, rb.velocity.y / 8, 0);
            }

        }

        if (climbingLedge)
        {
            if (isWallFront)
            {
                rb.AddForce(body.up * 15 + body.forward, ForceMode.Force);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 20, rb.velocity.z);
                rb.AddForce(body.up * 2 + body.forward * 4, ForceMode.Impulse);
                climbingLedge = false;
            }
        }
    }

    void CheckWall()
    {
        isWallLeft = Physics.BoxCast(wallCheck.position, new Vector3(0.1f, 0.7f, 0.1f), -body.right, out leftWallHit, body.rotation, wallDistance, walllayer);
        isWallRight = Physics.BoxCast(wallCheck.position, new Vector3(0.1f, 0.7f, 0.1f), body.right, out rightWallHit, body.rotation, wallDistance, walllayer);
        isWallFront = Physics.BoxCast(body.position, new Vector3(0.1f, 0.95f, 0.1f), body.forward, out frontWallHit, body.rotation, wallDistance, walllayer);
        isWallBack = Physics.BoxCast(wallCheck.position, new Vector3(0.1f, 0.7f, 0.1f), -body.forward, out backWallHit, body.rotation, wallDistance, walllayer);
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(body.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun()
    {
        if (!isWallrunning && isWallFront && rb.velocity.y > 1)
        {
            rb.AddForce((body.up + body.forward / 5) * wallFrontJumpForce * 100, ForceMode.Force);
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
                wallRunJumpDirection = body.up / 2 + leftWallHit.normal;
            }
            else if (isWallRight)
            {
                wallRunJumpDirection = body.up / 2 + rightWallHit.normal;
            }
            else if (isWallFront)
            {
                wallRunJumpDirection = body.up + frontWallHit.normal;
                rb.velocity = Vector3.zero;
                body.rotation = Quaternion.LookRotation(frontWallHit.normal);
            }
            else if (isWallBack)
            {
                wallRunJumpDirection = body.up + backWallHit.normal;
                rb.velocity = Vector3.zero;
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
        bool ledgeCheckUp = Physics.Raycast(ledgeUpCheck.position, body.forward, wallDistance, walllayer);
        bool ledgeCheckUp2 = Physics.Raycast(body.position, body.up, wallDistance * 3, walllayer);

        if (isWallFront && !ledgeCheckUp && !ledgeCheckUp2)
        {
            return true;
        }
        return false;
    }

    #endregion


    #region GunPull
    void GunPull()
    {
        if (gun.isPulling)
        {
            if (isWallFront || isHeadOnwall)
            {
                gun.isPulling = false;
                rb.useGravity = true;
                return;
            }
            if (rb.useGravity){rb.useGravity = false;} 
            rb.velocity = Vector3.zero;
            Vector3 hookShotDirection = (gun.pullPoint - transform.position).normalized;
            float hookShotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, gun.pullPoint), 10, 70);
            float hookShotSpeedMultiplier = 2f;

            rb.MovePosition(transform.position + hookShotDirection * hookShotSpeed * hookShotSpeedMultiplier * Time.fixedDeltaTime);

            float reachedHookShotPositionDistance = 0.6f;

            if (Vector3.Distance(transform.position, gun.pullPoint) < reachedHookShotPositionDistance
                || Vector3.Distance(groundCheck.position, gun.pullPoint) < reachedHookShotPositionDistance
                || Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), gun.pullPoint) < reachedHookShotPositionDistance
                || isWallFront)
            {
                gun.isPulling = false;
                //rb.velocity = Vector3.zero;
                rb.useGravity = true;
            }
        }
    }
    #endregion

    private void FixedUpdate()
    {
        MovePlayer();
        if (rb.useGravity)
        {
            rb.AddForce(Physics.gravity * rb.mass);
        }

        if (currentDirection != Vector3.zero && !gun.isAiming)
        {
            bodyRotation = Quaternion.LookRotation(currentDirection);
            body.rotation = Quaternion.Euler(body.rotation.eulerAngles.x, bodyRotation.eulerAngles.y, body.rotation.eulerAngles.z);
        }
        //if (rb.velocity != Vector3.zero && !gun.isAiming && !isWallrunning && !isWallFront)
        //{
        //    bodyRotation = Quaternion.LookRotation(rb.velocity);
        //    body.rotation = Quaternion.Euler(body.rotation.eulerAngles.x, bodyRotation.eulerAngles.y, body.rotation.eulerAngles.z);
        //}
        GunPull();
    }

    void MovePlayer()
    {
        if (!gun.isPulling)
        {
            if (isGrounded && !OnSlope() && !isSliding)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
                currentDirection = moveDirection.normalized;
            }
            else if (isGrounded && OnSlope() && !isSliding)
            {
                rb.AddForce(slopeDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
                currentDirection = slopeDirection.normalized;
            }
            else if (!isGrounded && !isSliding)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
                currentDirection = moveDirection.normalized;
            }
            else if (isSliding)
            {
                rb.AddForce(slidingDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
                currentDirection = slidingDirection.normalized;
            }
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerheight / 2 + 1f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (isWallLeft)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(wallCheck.position + -body.right * wallDistance, new Vector3(0.2f,1.4f, 0.2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(wallCheck.position + -body.right * wallDistance, new Vector3(0.2f,1.4f, 0.2f));
        }

        if (isWallRight)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(wallCheck.position + body.right * wallDistance, new Vector3(0.2f,1.4f, 0.2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(wallCheck.position + body.right * wallDistance, new Vector3(0.2f,1.4f, 0.2f));
        }

        if (isWallFront)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(body.position + body.forward * wallDistance, new Vector3(0.2f, 0.95f * 2, 0.2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(body.position + body.forward * wallDistance, new Vector3(0.2f, 0.95f * 2, 0.2f));
        }

        if (isWallBack)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(wallCheck.position + -body.forward * wallDistance, new Vector3(0.2f,1.4f, 0.2f));
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(wallCheck.position + -body.forward * wallDistance, new Vector3(0.2f,1.4f, 0.2f));
        }
    }
}
