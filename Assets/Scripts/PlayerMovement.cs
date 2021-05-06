using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMultiplier = 10f;
    public float airMultiplier = 0.4f;
    Vector3 moveDirection;
    Vector3 slopeDirection;
    float horizontalMovement;
    float verticalMovement;

    [Header("Sprinting")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;

    public Transform groundCheck;
    public LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.3F;

    RaycastHit slopeHit;
    float playerheight = 2f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        slopeDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }

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

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerheight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                Debug.Log("onslope");
                return true;
            }
        }
        return false;
    }
}
