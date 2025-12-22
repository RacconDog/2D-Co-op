using System;
using Unity.Netcode;
using UnityEngine;

public class AbstractStation : NetworkBehaviour
{
    [Header("Abstract")]
    [SerializeField] protected GameObject stationDevice;
    [SerializeField] public NetworkVariable<bool> isOccupied = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected virtual void Update()
    {
        // GameObject.Find("Station1").GetComponent<SpriteRenderer>().color = isOccupied.Value ? Color.red : Color.green;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetIsOccupiedServerRpc(bool state, ServerRpcParams rpcParams = default)
    {
        isOccupied.Value = state;
    }
    
    public virtual void StationAction(bool isPressedThisFrame)
    {
        Debug.LogError("::: Interact method not implemented");
    }
    public virtual void StationUpdateDir(Vector2 dir)
    {
        Debug.LogError("::: StationUpdateDir method not implemented");
    }
}
