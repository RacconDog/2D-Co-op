using System;
using UnityEngine;

public class AbstractStation : MonoBehaviour
{
    [Header("Abstract")]
    [SerializeField] protected GameObject STATION_DEVICE;

    // Local occupancy state
    public bool isOccupied = false;

    protected virtual void Update()
    {
        // // Optional debug: show occupancy color
        // if (STATION_DEVICE != null)
        //     STATION_DEVICE.GetComponent<Renderer>().material.color = isOccupied ? Color.red : Color.green;
    }

    public void SetIsOccupied(bool state)
    {
        isOccupied = state;
    }

    public virtual void StationAction(bool isPressedThisFrame)
    {
        Debug.LogWarning("::: StationAction method not implemented");
    }

    public virtual void StationUpdateDir(Vector2 dir)
    {
        Debug.LogWarning("::: StationUpdateDir method not implemented");
    }
}