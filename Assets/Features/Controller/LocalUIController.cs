using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class LocalUIController : AbstractController
{
    PlayerInput playerInput;

    InputAction selectAction;
    InputAction backAction;
    InputAction shiftRightAction;
    InputAction shiftLeftAction;

    protected override void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        selectAction = playerInput.actions["Select"];
        backAction = playerInput.actions["Back"];
        shiftRightAction = playerInput.actions["ShiftRight"];
        shiftLeftAction = playerInput.actions["ShiftLeft"];
    }

    void Start()
    {
        // playerID = playerInput.playerIndex;
        // Initialize(null, playerID); // no gameplay Player needed for UI

        playerID = playerInput.playerIndex;
    }


    void Update()
    {
        // Debug.Log($"Controller: {playerID} devices: {GetComponent<PlayerInput>().devices.Count}");
        input.UISelect = selectAction.WasPressedThisFrame();
        input.UIBack = backAction.WasPressedThisFrame();
        input.UIShiftRight = shiftRightAction.WasPressedThisFrame();
        input.UIShiftLeft = shiftLeftAction.WasPressedThisFrame();

        SendToMenuManager();

        input.ResetFrameInputs();
    }
}