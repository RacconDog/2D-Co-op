using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static bool gameStarted = false;

    [Header("Public Get References")]
    public GameObject playerPrefab;
    public GameObject GameContainer;
    public static Color[] curSkin = new Color[4];

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    public static PlayerManager Instance { get; private set; }

    public GameObject[] controllerList = new GameObject[4];
    public GameObject[] playerList = new GameObject[4];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame(GameObject game, GameObject ui)
    {
        game.SetActive(true);
        ui.SetActive(false);
    }

    void OnEnable()
    {
        if (PlayerInputManager.instance != null)
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    void OnDisable()
    {
        if (PlayerInputManager.instance != null)
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;

        // Safety check
        if (index < 0 || index >= controllerList.Length)
        {
            Debug.LogWarning("Player index out of bounds: " + index);
            return;
        }

        GameObject controller = playerInput.gameObject;
        controllerList[index] = controller;

        // Move to spawn point
        if (spawnPoints != null && spawnPoints.Length > index && spawnPoints[index] != null)
        {
            controller.transform.position = spawnPoints[index].position;
            controller.transform.rotation = spawnPoints[index].rotation;
        }

        // Lock control scheme so Unity doesn't reshuffle devices
        playerInput.neverAutoSwitchControlSchemes = true;

        // Optional: name for debugging
        controller.name = $"Controller {index + 1}";

        // // Debug info
        // Debug.Log($"Controller {index + 1} joined");
        // Debug.Log($" -> Devices: {playerInput.devices.Count}");

        foreach (var device in playerInput.devices)
        {
            Debug.Log($"    - {device.displayName}");
        }
    }

    public void RemoveController(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;

        if (index >= 0 && index < controllerList.Length)
        {
            controllerList[index] = null;
        }

        Destroy(playerInput.gameObject);
    }

    public GameObject GetController(int playerNumber)
    {
        if (playerNumber < 1 || playerNumber > controllerList.Length)
            return null;

        return controllerList[playerNumber - 1];
    }
}