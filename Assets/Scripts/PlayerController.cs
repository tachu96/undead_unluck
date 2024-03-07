using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveSpeedFactor;
    [SerializeField] private Transform orientation;
    [SerializeField] private float groundDrag;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    Vector3 moveDirection;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    private bool readyToJump;
    private bool jumpPressed;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;

    private bool grounded;

    // Code related to main unity functions goes here
    #region UnityFunctions
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Jump.canceled += Jump_canceled;

        playerInputActions.Player.Move.performed += Move_performed;
        playerInputActions.Player.Move.canceled += Move_canceled;

    }

    private void Start()
    {
        readyToJump = true;
    }

    #region InputSubscriptions
    private void Jump_performed(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        jumpPressed = false;
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        horizontalInput = 0;
        verticalInput = 0;
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;
    }
    #endregion

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight*0.5f*0.2f, groundMask);
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else 
        {
            rb.drag = 0f;
        }

        SpeedControl();
        JumpCheck();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    #endregion




    private void MovePlayer() 
    {
        moveDirection = orientation.forward * verticalInput + orientation.right*horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveSpeedFactor, ForceMode.Force);
        }
        else if (!grounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveSpeedFactor*airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed) { 
            Vector3 limitedVel=flatVel.normalized*moveSpeed;
            rb.velocity=new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void JumpCheck() {
        if (jumpPressed && readyToJump && grounded) { 
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump() { 
        readyToJump = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        float rayLength = playerHeight * 0.5f * 0.2f;

        Vector3 raycastOrigin = transform.position;

        bool grounded = Physics.Raycast(raycastOrigin, Vector3.down, rayLength, groundMask);

        if (grounded)
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawRay(raycastOrigin, Vector3.down * rayLength);
    }
}