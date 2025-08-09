using DeveloperConsole;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ConsoleManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    void Start()
    {
        Console.AddCommand("host", StartHostCommand);
        Console.AddCommand("join", StartJoinCommand);
        // Console.AddCommand("reload", StartReloadCommand);
    }

    void StartHostCommand(string[] args)
    {
        // print("hi");
        GetComponent<RelayManager>().CreateRelay();
    }

    void StartJoinCommand(string[] args)
    {
        GetComponent<RelayManager>().JoinRelay(args[0]);
    }

    // void StartReloadCommand(string[] args)
    // {
    //     string currentSceneName = SceneManager.GetActiveScene().name;
    //     SceneManager.LoadScene(currentSceneName);
    // }

    void Update()
    {
        if (inputActions.FindAction("Start").WasPressedThisFrame())
        {
            StartHostCommand(new string[] { });
        }

        // if (inputActions.FindAction("Reload").ReadValue<float>() == 1f)
        // {
        //     StartReloadCommand(new string[] { });
        //     Destroy(gameObject);
        //     // GetComponent<RelayManager>().EndRelay();
        // }
    }   

    // void OnEnable()
    // {
    //     Application.logMessageReceived += OnLogMessageReceived;
    // }

    // void OnDisable()
    // {
    //     Application.logMessageReceived -= OnLogMessageReceived;
    // }

    // void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    // {
    //     switch (type)
    //     {
    //         case LogType.Log:
    //             Print(condition);
    //             break;
    //         case LogType.Warning:
    //             PrintWarning(condition);
    //             break;
    //         case LogType.Error:
    //         case LogType.Exception:
    //         case LogType.Assert:
    //             PrintError(condition);
    //             break;
    //     }
    // }

    // void Print(string printString)
    // {
    //     Console.Print(printString);
    // }

    // void PrintWarning(string printString)
    // {
    //     Console.PrintWarning(printString);
    // }

    // void PrintError(string printString)
    // {
    //     Console.PrintError(printString);
    // }
}
