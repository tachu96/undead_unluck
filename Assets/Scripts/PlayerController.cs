using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Animator variables
    private const string ANIM_LIGHTATTACK1_BOOL = "lightAttack1";
    private const string ANIM_LIGHTATTACK2_BOOL = "lightAttack2";
    private const string ANIM_LIGHTATTACK3_BOOL = "lightAttack3";
    private const string ANIM_HEAVYATTACK1_BOOL = "heavyAttack1";
    private const string ANIM_HEAVYATTACK2_BOOL = "heavyAttack2";
    private const string ANIM_HEAVYATTACK3_BOOL = "heavyAttack3";


    //Animator states
    private const string ANIMSTATE_LIGHTATTACK1 = "LightAttack1";
    private const string ANIMSTATE_LIGHTATTACK2 = "LightAttack2";
    private const string ANIMSTATE_LIGHTATTACK3 = "LightAttack3";

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

    [Header("Fuuko Related References")]
    [SerializeField] private GameObject fuuko;
    [SerializeField] private FuukoBehaviour fuukoBehaviour;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private Transform releasePoint;
    [SerializeField] private GameObject carriedFuuko;
    [SerializeField] private float fuukoCallTimerMax;

    private bool fuukoInRange;
    private bool carryingFuuko;
    private bool calledFuuko=false;
    private bool runFuukoCallTimer;
    private float fuukoCallTimer;

    [Header("LightAttacks")]
    [SerializeField] private Animator animator;
    [SerializeField] private static int numberOfClicks=0;
    [SerializeField] private float maxComboDelay;

    private float lastClickedTime;


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

        playerInputActions.Player.Fuuko.started += Fuuko_started;
        playerInputActions.Player.Fuuko.canceled += Fuuko_canceled;

        playerInputActions.Player.LightAttack.started += LightAttack_started;
    }

    private void LightAttack_started(InputAction.CallbackContext context)
    {
        lastClickedTime = Time.time;
        numberOfClicks++;
        if (numberOfClicks==1) {
            animator.SetBool(ANIM_LIGHTATTACK1_BOOL, true);
        }
        numberOfClicks = Mathf.Clamp(numberOfClicks, 0, 3);

        if (numberOfClicks >= 2&& animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMSTATE_LIGHTATTACK1)) { 
            animator.SetBool(ANIM_LIGHTATTACK1_BOOL, false);
            animator.SetBool(ANIM_LIGHTATTACK2_BOOL, true);
        }

        if (numberOfClicks >= 3 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMSTATE_LIGHTATTACK2))
        {
            animator.SetBool(ANIM_LIGHTATTACK2_BOOL, false);
            animator.SetBool(ANIM_LIGHTATTACK3_BOOL, true);
        }
    }

    private void Fuuko_started(InputAction.CallbackContext context)
    {
        //if Fuuko is near, lets try to carry Fuuko
        if (fuukoInRange)
        {
            fuukoInRange = false;
            carryingFuuko = true;
            CarryFuuko();
        }
        else if (carryingFuuko) {
            carryingFuuko = false;
            ReleaseFuuko();
        }

        if (!carryingFuuko)
        {
            runFuukoCallTimer = true;
        }
    }

    private void Fuuko_canceled(InputAction.CallbackContext context)
    {
        fuukoCallTimerReset();
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

    private void Start()
    {
        readyToJump = true;
    }

    private void Update()
    {
        Debug.Log(numberOfClicks);

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f * 0.2f, groundMask);
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }

        if (runFuukoCallTimer) {
            FuukoCallControl();
        }

        HandleAttackCombo();

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
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveSpeedFactor, ForceMode.Force);
        }
        else if (!grounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * moveSpeedFactor * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl() {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed) {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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

    public void FuukoEnteredRange() {
        fuukoInRange = true;
        calledFuuko = false;
        fuukoBehaviour.CancelCallFuukoToPlayer();
        fuukoCallTimerReset();
    }
    public void FuukoExitedRange()
    {
        fuukoInRange = false;
    }
    private void CarryFuuko() {
        calledFuuko = false;
        fuukoCallTimerReset();
        fuukoBehaviour.PrepareForCarry();
        carriedFuuko.SetActive(true);
    }

    private void ReleaseFuuko() {

        fuukoCallTimerReset();
        calledFuuko = false;

        carriedFuuko.SetActive(false);
        fuuko.transform.position = releasePoint.position;
        fuuko.transform.rotation = releasePoint.rotation;
        fuukoBehaviour.PrepareForRelease();
    }

    private void FuukoCallControl() {
        fuukoCallTimer += Time.deltaTime;

        if (fuukoCallTimer>=fuukoCallTimerMax) {
            fuukoCallTimerReset();
            calledFuuko =!calledFuuko;

            if (calledFuuko)
            {
                fuukoBehaviour.CallFuukoToPlayer();
            }
            else {
                fuukoBehaviour.CancelCallFuukoToPlayer();
            }
        }
    }

    private void fuukoCallTimerReset() { 
        fuukoCallTimer = 0;
        runFuukoCallTimer = false;
    }

    private void HandleAttackCombo() {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMSTATE_LIGHTATTACK1))
        {
            animator.SetBool(ANIM_LIGHTATTACK1_BOOL, false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMSTATE_LIGHTATTACK2))
        {
            animator.SetBool(ANIM_LIGHTATTACK2_BOOL, false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMSTATE_LIGHTATTACK3))
        {
            animator.SetBool(ANIM_LIGHTATTACK3_BOOL, false);
            numberOfClicks = 0;
        }

        if (Time.time-lastClickedTime>maxComboDelay) { 
            Debug.Log("AttackDelayMissed");
            numberOfClicks = 0;
        }

    }
}