using Unity.Mathematics;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    public Grid grid;
    [SerializeField] float radius = 0;
    [SerializeField] Vector2 pos = Vector2.zero;
    [SerializeField] float strength;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            grid.ApplyDirectedForce(Vector2.left * strength, pos, radius);
        }
    }
}
