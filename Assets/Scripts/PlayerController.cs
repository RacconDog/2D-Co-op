using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float moveSpeedCap = 10f;
    [SerializeField] float moveSpeedDamp = 10f;
    [SerializeField][Range(0, 1)] float moveDragFactor = 10f;
    [SerializeField] float jumpBufferLength = 10f;
    float jumpBufferCurTime = 0f;
    [SerializeField] float jumpSpeed = 5f;

    [SerializeField] InputActionAsset inputActions;
    InputAction moveAction;
    InputAction jumpAction;

    Rigidbody2D rb;

    [SerializeField] float groundCheckDistance = 0.1f;

    [SerializeField] LayerMask groundLayer;

    GroundState curGroundState = GroundState.Airborne;
    GroundState lastFrameGroundState = GroundState.Airborne;
    enum GroundState
    {
        Airborne = 0,
        Grounded = 1
    }

    [SerializeField] Animator animator;


    void Start()
    {
        if (!IsOwner) return;

        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();

        jumpBufferCurTime = jumpBufferLength;
    }

    void Update()
    {
        curGroundState = CheckGroundState();
        animator.SetBool("isGrounded", Convert.ToBoolean(curGroundState));

        if (curGroundState != lastFrameGroundState)
        {
            // print("is owner: " + IsOwner + " | " + transform.position.y);
            print(curGroundState + " | " + lastFrameGroundState);
        }

        if (!IsOwner) return;

        // if (curGroundState != lastFrameGroundState)
        // {
        //     UpdateGroundedClientRpc(curGroundState);
        // }

        //Calculate Movement Force
        Vector3 moveforce = Vector3.right * moveAction.ReadValue<Vector2>().x * moveSpeed;  // Initial movement force
        rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -moveSpeedCap, moveSpeedCap);  // Clamp horizontal velocity
        if (moveAction.ReadValue<Vector2>().x == 0) rb.linearVelocityX -= CalculateDrag();   // Apply drag to horizontal velocity


        // Jump Buffer Logic
        if (curGroundState == GroundState.Airborne)
        {
            jumpBufferCurTime -= Time.deltaTime;

            if (jumpAction.WasPressedThisFrame())
            {
                jumpBufferCurTime = jumpBufferLength;
            }
        }

        bool groundedJump = curGroundState == GroundState.Grounded && jumpAction.WasPressedThisFrame();                                         //Logic for when you press jump on ground
        bool bufferJump = curGroundState == GroundState.Grounded && lastFrameGroundState == GroundState.Airborne && jumpBufferCurTime > 0f;     //Logic for when you press jump right before you hit ground

        //Calculate Jump Force
        Vector3 jumpForce = Vector3.zero;
        if (groundedJump || bufferJump)
        {
            rb.linearVelocityY = 0f;
            jumpForce = Vector3.up * jumpSpeed;
        }

        //Apply Calculated Forces
        rb.AddForce(moveforce, ForceMode2D.Force);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);


        // Update Animator Vars
        // animator.SetFloat("yVelo", rb.linearVelocityY);
    }

    void LateUpdate()
    {
        //set Last frames groundstate
        lastFrameGroundState = curGroundState;
    }

    GroundState CheckGroundState()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, transform.position.y - (GetComponent<BoxCollider2D>().bounds.size.y / 2)),
            Vector2.down,
            groundCheckDistance,
            groundLayer);

        if (hit)
        {
            // print(hit.transform.gameObject.name);
            return GroundState.Grounded;
        }
        return GroundState.Airborne;
    }

    float CalculateDrag()
    {
        if (moveDragFactor <= 0f) return 0f;

        float k = Mathf.Log(2f) / moveDragFactor;
        float decay = Mathf.Exp(-k * Time.deltaTime);
        float newVelocityX = rb.linearVelocityX * decay;

        return rb.linearVelocityX - newVelocityX;
    }

    // //-----Network Stuff-----\\
    // [ClientRpc] void UpdateGroundedClientRpc(GroundState state)
    // {
    //     if (!IsOwner) // Don’t overwrite our own animator
    //         animator.SetBool("isGrounded", Convert.ToBoolean(state));
    // }
}
