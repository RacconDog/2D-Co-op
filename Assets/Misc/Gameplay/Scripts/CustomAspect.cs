using UnityEngine;

public class CustomAspect : MonoBehaviour
{
    void Start()
    {
        GetComponent<Camera>().aspect = 1;
    }
}
