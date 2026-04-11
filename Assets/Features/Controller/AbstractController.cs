using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public abstract class AbstractController : MonoBehaviour
{
    [Header("References")]
    protected PlayerInputData input;
    protected int playerID;

    // static registry of controllers in scene
    private static List<AbstractController> controllers = new();

    // =========================
    // AUTO ASSIGN ID
    // =========================
    protected virtual void Awake()
    {
        controllers.Add(this);

        playerID = controllers.Count;
        gameObject.name = $"Controller {playerID}";

        Debug.Log($"Assigned PlayerID {playerID} to {name}");
    }

    protected virtual void OnDestroy()
    {
        controllers.Remove(this);

        // Recalculate IDs (keeps ordering clean)
        for (int i = 0; i < controllers.Count; i++)
        {
            controllers[i].playerID = i + 1;
            controllers[i].gameObject.name = $"Controller {i + 1}";
        }
    }

    // =========================
    // INIT ENTRY
    // =========================
    
    public void SpawnPlayer()
    {
        Transform container = PlayerManager.Instance.GameContainer.transform;

        // Spawn player
        GameObject playerGO = Instantiate(PlayerManager.Instance.playerPrefab, container);
        PlayerManager.Instance.playerList[playerID] = playerGO;

        playerGO.GetComponent<Player>().PLAYER_RENDERER.material.color = PlayerManager.curSkin[playerID];
        

        GetComponent<PlayerInput>().actions.Disable(); // 🔥 hard reset
        GetComponent<PlayerInput>().currentActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        GetComponent<PlayerInput>().actions.Enable();


        PlayerManager.gameStarted = true;
    }

    // =========================
    // INPUT
    // =========================
    protected void SendToPlayer()
    {
        if (PlayerManager.Instance.playerList[playerID] == null) return;
        input.playerID = playerID;
        PlayerManager.Instance.playerList[playerID].GetComponent<Player>().SetInput(input);
    }

    protected void SendToMenuManager()
    {
        if (MenuManager.Instance == null) return;

        input.playerID = playerID;
        MenuManager.Instance.SetInput(input);
    }
}

    
public struct PlayerInputData
{
    public int playerID;

    public Vector2 move;
    public bool jump;
    public bool interact;
    public Vector2 aim;
    public bool stationHeld;
    public bool animTest;

    public bool UISelect;
    public bool UIBack;
    public bool UIShiftRight;
    public bool UIShiftLeft;

    public void ResetFrameInputs()
    {
        jump = false;
        interact = false;
        animTest = false;
        UISelect = false;
        UIBack = false;
        UIShiftRight = false;
        UIShiftLeft = false;
        // move, aim, stationHeld are continuous inputs, usually not reset
    }
}
