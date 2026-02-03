using Unity.Mathematics;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    public Grid grid;
    [SerializeField] float radius = 0;
    [SerializeField] Vector2 pos = Vector2.zero;
    [SerializeField] float strength;
    [SerializeField] float frequency = 1f;
    
    float time = 0;

    void Update()
    {

        if (time > 1f / frequency)
        {
            grid.ApplyExplosiveForce(strength, pos, radius);
            time = 0;
        }
        time += Time.deltaTime;
    }
}
