using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
// using Console = DeveloperConsole.Console;

public class RelayManager : MonoBehaviour
{
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            print("Youve been Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            print("Lobby Code: " + joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            print(e.ToString());
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            print("Joining relay " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
            (
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            print(e.ToString());
        }
    }

//     public void EndRelay()
//     {
//         if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
//         {
//             Console.Print("Shutting down host/server...");
//         }
//         else if (NetworkManager.Singleton.IsClient)
//         {
//             Console.Print("Disconnecting client...");
//         }
    
//         // Shutdown Netcode for GameObjects
//         NetworkManager.Singleton.Shutdown();
    
//         // Optionally reset transport data (not strictly necessary but useful if you restart session)
//         UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
//         if (transport != null)
//         {
//             transport.SetConnectionData("127.0.0.1", 7777); // dummy fallback data
//         }
    
//         Console.Print("Relay session ended.");
// }   
}
