using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System;
using Unity.VisualScripting;
using NUnit.Framework;
using RangeAttribute = UnityEngine.RangeAttribute;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float moveSpeedCap = 10f;
    [SerializeField][Range(0, 1)] float moveDragFactor = 10f;

    [Header("Jump")]
    [SerializeField] float jumpBufferLength = 10f;
    float jumpBufferCurTime = 0f;
    [SerializeField] float jumpSpeed = 5f;

    [Header("References")]
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] Renderer playerRenderer;
    [SerializeField] Animator animator;
    Transform playerSpawnPoint;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction animTestAction;
    InputAction interactAction;
    InputAction stationAim;
    InputAction StationAction; 

    Rigidbody2D rb;

    [Header("Ground Check")]
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GroundState curGroundState = GroundState.Airborne;
    [SerializeField] GroundState lastFrameGroundState = GroundState.Airborne;
    public enum GroundState
    {
        Airborne = 0,
        Grounded = 1
    }

    [Header("Interaction")]
    [SerializeField] float interactionDistance = 100f;
    [SerializeField] GameObject curStation;
    GameObject[] stationList;

    [Header("Misc")]
    PlayerState playerState = PlayerState.Moving;
    enum PlayerState
    {
        AtStation,
        Moving
    }

    //internal
    Vector2 lastShipPosition = Vector2.zero;

    public override void OnNetworkSpawn()
    {
        int playerIndex = (int)GetComponent<NetworkObject>().OwnerClientId;
        playerRenderer.material.color = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().playerColors[playerIndex];


        playerSpawnPoint = GameObject.Find("PlayerSpawn").transform;
        transform.position = playerSpawnPoint.position;
    }

    void Start()
    {
        if (!IsOwner) return;

        moveAction = inputActions.FindAction("Move");
        jumpAction = inputActions.FindAction("Jump");
        animTestAction = inputActions.FindAction("AnimTest");
        interactAction = inputActions.FindAction("Interact");
        stationAim = inputActions.FindAction("StationAim");
        StationAction = inputActions.FindAction("StationAction");

        rb = GetComponent<Rigidbody2D>();
        stationList = GameObject.FindGameObjectsWithTag("Station");

        jumpBufferCurTime = jumpBufferLength;

    }

    void Update()
    {
        //Check and manage ground states for the local and remote player
        curGroundState = CheckGroundState();
        animator.SetBool("isGrounded", Convert.ToBoolean(curGroundState));
        animator.SetFloat("speedc", Mathf.Abs(rb.linearVelocity.x));

        if (!IsOwner) return;

        if (playerState == PlayerState.AtStation)
            AtStation();
            
        else if (playerState == PlayerState.Moving)
            Movement();
    }

    void AtStation()
    {
        // Exit Station
        if (interactAction.WasPressedThisFrame())
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            playerState = PlayerState.Moving;

            curStation.GetComponent<AbstractStation>().SetIsOccupiedServerRpc(false);
            curStation = null;
        }

        // Station Logic
        if (curStation != null)
        {
            curStation.GetComponent<AbstractStation>().StationUpdateDir(stationAim.ReadValue<Vector2>());
            if (StationAction.IsPressed())
                curStation.GetComponent<AbstractStation>().StationAction(StationAction.WasPressedThisFrame());
        }
    }

    void Movement()
    {
        if (IsOwner)
        {
            // Interaction Logic
            if (interactAction.WasPressedThisFrame())
            {
                GameObject closestStation = null;
                float closestStationDist = 9999f;

                foreach (GameObject station in stationList)
                {
                    float curStationDist = Vector2.Distance(transform.position, station.transform.position);
                    if (curStationDist < closestStationDist && curStationDist < interactionDistance && station.GetComponent<AbstractStation>().isOccupied.Value == false)
                    {
                        closestStation = station;
                        closestStationDist = curStationDist;
                    }
                }

                //EnterStation
                if (closestStation != null)
                {
                    playerState = PlayerState.AtStation;
                    transform.position = closestStation.transform.position;
                    // rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;

                    curStation = closestStation;
                    curStation.GetComponent<AbstractStation>().SetIsOccupiedServerRpc(true);
                    return;
                }
            }
        }
        
        if (animTestAction.WasPressedThisFrame())
        {
            animator.SetTrigger("testAnim");
        }

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
    }

    void LateUpdate()
    {
        //set Last frames groundstate
        lastFrameGroundState = curGroundState;
    }

    GroundState CheckGroundState()
    {
        // left side ground check
        RaycastHit2D hitL = Physics2D.Raycast(
            new Vector2(transform.position.x - (GetComponent<CapsuleCollider2D>().bounds.size.x / 2), transform.position.y - (GetComponent<CapsuleCollider2D>().bounds.size.y / 2)),
            Vector2.down,
            groundCheckDistance,
            groundLayer);

        //right side ground check
        RaycastHit2D hitR = Physics2D.Raycast(
            new Vector2(transform.position.x + (GetComponent<CapsuleCollider2D>().bounds.size.x / 2), transform.position.y - (GetComponent<CapsuleCollider2D>().bounds.size.y / 2)),
            Vector2.down,
            groundCheckDistance,
            groundLayer);

        if (hitL || hitR)
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
