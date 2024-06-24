using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNew : MonoBehaviour


{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 10f; // Adjust jump force as needed
    public float ODMMovementSpeed = 50f;

    [Header("Ground Check")]
    public float drag = 5f;
    public float airMultiplier = 0.1f;

    [Header("References")]
    public Transform groundCheck;
    public LayerMask ground;

    private Rigidbody rb;
    public Transform orientation;
    private bool isGrounded;

    public bool activeGrapple;
    public bool freezMovement;
    public bool isSwingingLeft;
    public bool isSwingingRight;
    public bool isSliding;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Debug.Log("Speed is : " + rb.velocity.magnitude + " || " + rb.velocity  );

        if (freezMovement)
        {
            // rb.velocity = Vector3.zero;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (!isGrounded) OdmMovement();

        if (Input.GetKeyDown(KeyCode.K))
        {
            gameObject.transform.position = new Vector3(0f, 1.79f, 0f);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyDrag();
    }

    private void MovePlayer()
    {
        if (isSliding) return;
        if (activeGrapple) return;
        if (isSwingingLeft || isSwingingRight) return;


        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0f; // Ensure movement is horizontal

        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void ApplyDrag()
    {
        rb.drag = isGrounded && !activeGrapple ? drag : 0;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OdmMovement()
    {
        if (Input.GetKeyDown(KeyCode.A)) rb.AddForce(orientation.right * ODMMovementSpeed * Time.deltaTime, ForceMode.Force);
        if (Input.GetKeyDown(KeyCode.D)) rb.AddForce(-orientation.right * ODMMovementSpeed * Time.deltaTime, ForceMode.Force);
    }

}
