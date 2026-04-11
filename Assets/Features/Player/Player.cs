using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float MOVE_SPEED = 5f;
    [SerializeField] float MOVE_SPEED_CAP = 10f;
    [SerializeField][Range(0, 1)] float MOVE_DRAG_FACTOR = 10f;

    [Header("Jump")]
    [SerializeField] float JUMP_BUFFER_LENGTH = 0.2f;
    [SerializeField] float JUMP_SPEED = 5f;
    float jumpBufferCurTime = 0f;

    [Header("References")]
    [SerializeField] Animator ANIMATOR;
    [SerializeField] public Renderer PLAYER_RENDERER;

    Rigidbody2D RB;
    BoxCollider2D boxCollider;

    [Header("Ground Check")]
    [SerializeField] float GROUND_CHECK_DISTANCE = 0.1f;
    [SerializeField] LayerMask GROUND_LAYER;

    GroundState curGroundState = GroundState.Airborne;
    GroundState lastFrameGroundState = GroundState.Airborne;

    public enum GroundState
    {
        Airborne = 0,
        Grounded = 1
    }

    [Header("Interaction")]
    [SerializeField] float INTERACTION_DISTANCE = 100f;

    GameObject curStation;
    GameObject[] stationList;

    enum PlayerState
    {
        AtStation,
        Moving
    }

    [SerializeField] PlayerState playerState = PlayerState.Moving;

    // =========================
    // INPUT (SET BY CONTROLLER)
    // =========================
    PlayerInputData input;

    public void SetInput(PlayerInputData newInput)
    {
        input = newInput;
    }

    // =========================
    // Unity Callbacks
    // =========================
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Spawn at default location
        Transform spawnPoint = GameObject.Find("PlayerSpawn")?.transform;
        if (spawnPoint != null)
            transform.position = spawnPoint.position;

        stationList = GameObject.FindGameObjectsWithTag("Station");

        jumpBufferCurTime = JUMP_BUFFER_LENGTH;
    }

    void Update()
    {
        curGroundState = CheckGroundState();
        ANIMATOR.SetBool("isGrounded", curGroundState == GroundState.Grounded);

        if (playerState == PlayerState.AtStation)
            HandleStation();
        else
            HandleMovement();
    }

    void LateUpdate()
    {
        lastFrameGroundState = curGroundState;
    }

    // =========================
    // Movement / Jump / Drag
    // =========================
    void HandleMovement()
    {
        // Station interaction
        if (input.interact)
        {
            TryEnterStation();
            if (playerState == PlayerState.AtStation) return;
        }

        // Anim test
        if (input.animTest)
            ANIMATOR.SetTrigger("testAnim");

        // Horizontal movement
        float targetVelX = input.move.x * MOVE_SPEED;

        RB.linearVelocity = new Vector2(
            Mathf.Clamp(RB.linearVelocity.x + (targetVelX - RB.linearVelocity.x),
                        -MOVE_SPEED_CAP, MOVE_SPEED_CAP),
            RB.linearVelocity.y
        );

        // Drag when no input
        if (input.move.x == 0)
        {
            RB.linearVelocity = new Vector2(
                RB.linearVelocity.x - CalculateDrag(),
                RB.linearVelocity.y
            );
        }

        // Jump buffer
        if (curGroundState == GroundState.Airborne)
        {
            jumpBufferCurTime -= Time.deltaTime;

            if (input.jump)
                jumpBufferCurTime = JUMP_BUFFER_LENGTH;
        }

        bool groundedJump = curGroundState == GroundState.Grounded && input.jump;

        bool bufferJump =
            curGroundState == GroundState.Grounded &&
            lastFrameGroundState == GroundState.Airborne &&
            jumpBufferCurTime > 0f;

        if (groundedJump || bufferJump)
        {
            RB.linearVelocity = new Vector2(RB.linearVelocity.x, 0f);
            RB.AddForce(Vector2.up * JUMP_SPEED, ForceMode2D.Impulse);
        }
    }

    float CalculateDrag()
    {
        if (MOVE_DRAG_FACTOR <= 0f) return 0f;

        float k = Mathf.Log(2f) / MOVE_DRAG_FACTOR;
        float decay = Mathf.Exp(-k * Time.deltaTime);
        float newVel = RB.linearVelocity.x * decay;

        return RB.linearVelocity.x - newVel;
    }

    // =========================
    // Station Logic
    // =========================
    void HandleStation()
    {
        if (curStation == null)
        {
            playerState = PlayerState.Moving;
            return;
        }

        var station = curStation.GetComponent<AbstractStation>();
        station.StationUpdateDir(input.aim);

        if (input.stationHeld)
            station.StationAction(input.jump);

        if (input.interact) // exit
        {
            RB.bodyType = RigidbodyType2D.Dynamic;
            playerState = PlayerState.Moving;

            station.SetIsOccupied(false);
            curStation = null;
        }
    }

    void TryEnterStation()
    {
        GameObject closestStation = null;
        float closestDist = float.MaxValue;

        foreach (GameObject station in stationList)
        {
            if (station.GetComponent<AbstractStation>().isOccupied)
                continue;

            float dist = Vector2.Distance(transform.position, station.transform.position);

            if (dist < INTERACTION_DISTANCE && dist < closestDist)
            {
                closestStation = station;
                closestDist = dist;
            }
        }

        if (closestStation != null)
        {
            curStation = closestStation;
            playerState = PlayerState.AtStation;

            transform.position = closestStation.transform.position;
            RB.bodyType = RigidbodyType2D.Static;

            curStation.GetComponent<AbstractStation>().SetIsOccupied(true);
        }
    }

    // =========================
    // Ground Check
    // =========================
    GroundState CheckGroundState()
    {
        Vector2 left = new Vector2(
            transform.position.x - boxCollider.bounds.extents.x,
            transform.position.y - boxCollider.bounds.extents.y
        );

        Vector2 right = new Vector2(
            transform.position.x + boxCollider.bounds.extents.x,
            transform.position.y - boxCollider.bounds.extents.y
        );

        RaycastHit2D hitL = Physics2D.Raycast(left, Vector2.down, GROUND_CHECK_DISTANCE, GROUND_LAYER);
        RaycastHit2D hitR = Physics2D.Raycast(right, Vector2.down, GROUND_CHECK_DISTANCE, GROUND_LAYER);

        return (hitL || hitR) ? GroundState.Grounded : GroundState.Airborne;
    }
}