using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] float delay = 1f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
