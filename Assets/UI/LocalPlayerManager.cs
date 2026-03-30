using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerManager : MonoBehaviour
{

    public static LocalPlayerManager Instance;

    private List<PlayerController> players = new();

    void Start()
    {
        
    }

    public void RemovePlayer(PlayerController player)
    {
        players.Remove(player);

        Destroy(player.gameObject);
    }
}
