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
    [SerializeField] float MOVE_SPEED = 5f;
    [SerializeField] float MOVE_SPEED_CAP = 10f;
    [SerializeField][Range(0, 1)] float MOVE_DRAG_FACTOR = 10f;

    [Header("Jump")]
    [SerializeField] float JUMP_BUFFER_LENGTH = 10f;
    float jumpBufferCurTime = 0f;
    [SerializeField] float JUMP_SPEED = 5f;

    [Header("References")]
    [SerializeField] InputActionAsset INPUT_ACTIONS;
    [SerializeField] Renderer PLAYER_RENDERER;
    [SerializeField] Animator ANIMATOR;
    Transform PLAYER_SPAWN_POINT;

    InputAction MOVE_ACTION;
    InputAction JUMP_ACTION;
    InputAction ANIM_TEST_ACTION;
    InputAction INTERACT_ACTION;
    InputAction STATION_AIM;
    InputAction STATION_ACTION; 

    Rigidbody2D RB;

    [Header("Ground Check")]
    [SerializeField] float GROUND_CHECK_DISTANCE = 0.1f;
    [SerializeField] LayerMask GROUND_LAYER;
    [SerializeField] GroundState curGroundState = GroundState.Airborne;
    [SerializeField] GroundState lastFrameGroundState = GroundState.Airborne;
    public enum GroundState
    {
        Airborne = 0,
        Grounded = 1
    }

    [Header("Interaction")]
    [SerializeField] float INTERACTION_DISTANCE = 100f;
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
        PLAYER_RENDERER.material.color = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().PLAYER_COLORS[playerIndex];


        PLAYER_SPAWN_POINT = GameObject.Find("PlayerSpawn").transform;
        transform.position = PLAYER_SPAWN_POINT.position;
    }

    void Start()
    {
        if (!IsOwner) return;

        MOVE_ACTION = INPUT_ACTIONS.FindAction("Move");
        JUMP_ACTION = INPUT_ACTIONS.FindAction("Jump");
        ANIM_TEST_ACTION = INPUT_ACTIONS.FindAction("AnimTest");
        INTERACT_ACTION = INPUT_ACTIONS.FindAction("Interact");
        STATION_AIM = INPUT_ACTIONS.FindAction("StationAim");
        STATION_ACTION = INPUT_ACTIONS.FindAction("StationAction");

        RB = GetComponent<Rigidbody2D>();
        stationList = GameObject.FindGameObjectsWithTag("Station");

        jumpBufferCurTime = JUMP_BUFFER_LENGTH;

    }

    void Update()
    {
        //Check and manage ground states for the local and remote player
        curGroundState = CheckGroundState();
        ANIMATOR.SetBool("isGrounded", Convert.ToBoolean(curGroundState));

        if (!IsOwner) return;

        if (playerState == PlayerState.AtStation)
            AtStation();
            
        else if (playerState == PlayerState.Moving)
            Movement();
    }

    void AtStation()
    {
        // Exit Station
        if (INTERACT_ACTION.WasPressedThisFrame())
        {
            RB.bodyType = RigidbodyType2D.Dynamic;
            playerState = PlayerState.Moving;

            curStation.GetComponent<AbstractStation>().SetIsOccupiedServerRpc(false);
            curStation = null;
        }

        // Station Logic
        if (curStation != null)
        {
            curStation.GetComponent<AbstractStation>().StationUpdateDir(STATION_AIM.ReadValue<Vector2>());
            if (STATION_ACTION.IsPressed())
                curStation.GetComponent<AbstractStation>().StationAction(STATION_ACTION.WasPressedThisFrame());
        }
    }

    void Movement()
    {
        if (IsOwner)
        {
            // Interaction Logic
            if (INTERACT_ACTION.WasPressedThisFrame())
            {
                GameObject closestStation = null;
                float closestStationDist = 9999f;

                foreach (GameObject station in stationList)
                {
                    float curStationDist = Vector2.Distance(transform.position, station.transform.position);
                    if (curStationDist < closestStationDist && curStationDist < INTERACTION_DISTANCE && station.GetComponent<AbstractStation>().isOccupied.Value == false)
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
                    RB.bodyType = RigidbodyType2D.Static;

                    curStation = closestStation;
                    curStation.GetComponent<AbstractStation>().SetIsOccupiedServerRpc(true);
                    return;
                }
            }
        }
        
        if (ANIM_TEST_ACTION.WasPressedThisFrame())
        {
            ANIMATOR.SetTrigger("testAnim");
        }

        //Calculate Movement Force
        Vector3 moveforce = Vector3.right * MOVE_ACTION.ReadValue<Vector2>().x * MOVE_SPEED;  // Initial movement force
        RB.linearVelocityX = Mathf.Clamp(RB.linearVelocityX, -MOVE_SPEED_CAP, MOVE_SPEED_CAP);  // Clamp horizontal velocity
        if (MOVE_ACTION.ReadValue<Vector2>().x == 0) RB.linearVelocityX -= CalculateDrag();   // Apply drag to horizontal velocity


        // Jump Buffer Logic
        if (curGroundState == GroundState.Airborne)
        {
            jumpBufferCurTime -= Time.deltaTime;

            if (JUMP_ACTION.WasPressedThisFrame())
            {
                jumpBufferCurTime = JUMP_BUFFER_LENGTH;
            }
        }

        bool groundedJump = curGroundState == GroundState.Grounded && JUMP_ACTION.WasPressedThisFrame();                                         //Logic for when you press jump on ground
        bool bufferJump = curGroundState == GroundState.Grounded && lastFrameGroundState == GroundState.Airborne && jumpBufferCurTime > 0f;     //Logic for when you press jump right before you hit ground

        //Calculate Jump Force
        Vector3 jumpForce = Vector3.zero;
        if (groundedJump || bufferJump)
        {
            RB.linearVelocityY = 0f;
            jumpForce = Vector3.up * JUMP_SPEED;
        }

        //Apply Calculated Forces
        RB.AddForce(moveforce, ForceMode2D.Force);
        RB.AddForce(jumpForce, ForceMode2D.Impulse);
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
            GROUND_CHECK_DISTANCE,
            GROUND_LAYER);

        //right side ground check
        RaycastHit2D hitR = Physics2D.Raycast(
            new Vector2(transform.position.x + (GetComponent<CapsuleCollider2D>().bounds.size.x / 2), transform.position.y - (GetComponent<CapsuleCollider2D>().bounds.size.y / 2)),
            Vector2.down,
            GROUND_CHECK_DISTANCE,
            GROUND_LAYER);

        if (hitL || hitR)
        {
            // print(hit.transform.gameObject.name);
            return GroundState.Grounded;
        }
        return GroundState.Airborne;
    }

    float CalculateDrag()
    {
        if (MOVE_DRAG_FACTOR <= 0f) return 0f;

        float k = Mathf.Log(2f) / MOVE_DRAG_FACTOR;
        float decay = Mathf.Exp(-k * Time.deltaTime);
        float newVelocityX = RB.linearVelocityX * decay;

        return RB.linearVelocityX - newVelocityX;
    }

    // //-----Network Stuff-----\\
    // [ClientRpc] void UpdateGroundedClientRpc(GroundState state)
    // {
    //     if (!IsOwner) // Don’t overwrite our own animator
    //         animator.SetBool("isGrounded", Convert.ToBoolean(state));
    // }
}
