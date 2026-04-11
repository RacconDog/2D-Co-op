using UnityEngine;

public class StationManager : MonoBehaviour
{
    public static StationManager Instance;

    void Awake()
    {
        Instance = this;
    }
}