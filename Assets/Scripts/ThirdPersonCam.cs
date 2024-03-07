using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;

    [Header("References")]
    [SerializeField]private Transform orientation;
    [SerializeField]private Transform player;
    [SerializeField]private Transform playerObj;
    [SerializeField]private Rigidbody rb;

    [Header("RotationAdjustment")]
    [SerializeField] private float rotationSpeed;

    private void Awake()
    {
        PlayerInputActions playerInputActions= new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Move.performed += Move_performed;
        playerInputActions.Player.Move.canceled += Move_canceled;
    }

    private void Move_canceled(InputAction.CallbackContext context)
    {
        horizontalInput = 0;
        verticalInput = 0;
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector =context.ReadValue<Vector2>();
        horizontalInput=inputVector.x;
        verticalInput=inputVector.y;
    }

    private void Update()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        Vector3 inputDir=orientation.forward*verticalInput+orientation.right*horizontalInput;

        if (inputDir != Vector3.zero) {
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
    }
}
