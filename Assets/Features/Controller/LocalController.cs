using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class LocalController : AbstractController
{
    PlayerInput playerInput;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction interactAction;
    InputAction aimAction;
    InputAction stationAction;
    InputAction animTestAction;

    protected override void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        interactAction = playerInput.actions["Interact"];
        aimAction = playerInput.actions["StationAim"];
        stationAction = playerInput.actions["StationAction"];
        animTestAction = playerInput.actions["AnimTest"];
    }

    void Start()
    {
        playerID = playerInput.playerIndex;

        // Player spawnedPlayer = PlayerManager.Instance.SpawnPlayer(playerID);
        // Initialize(spawnedPlayer, playerID);
    }

    void Update()
    {   
        // if (PlayerManager.gameStarted)
        // {
        //     GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
        //     print(GetComponent<PlayerInput>().currentActionMap.name);
        // }

        print(GetComponent<PlayerInput>().currentActionMap.name);

        input.move = moveAction.ReadValue<Vector2>();
        input.jump = jumpAction.WasPressedThisFrame();
        input.interact = interactAction.WasPressedThisFrame();
        input.aim = aimAction.ReadValue<Vector2>();
        input.stationHeld = stationAction.IsPressed();
        input.animTest = animTestAction.WasPressedThisFrame();

        SendToPlayer();
        print(input.move);

        input.ResetFrameInputs();
    }
}