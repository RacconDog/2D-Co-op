using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    [SerializeField] Vector2Int size = new Vector2Int(10, 10);
    [SerializeField] Vector2 spacing = Vector2.one;

    public PointMass[,] points { get; private set; }
    PointMass[,] fixedPoints;

    Spring[] springs;

    int originX;
    int originY;

    void Awake()
    {
        CreateGrid();
    }

    void Update()
    {
        // Update springs
        foreach (var s in springs)
            s.UpdateSpring();

        // Update points
        foreach (var p in points)
            p.UpdatePoint();

        // Example: shift the grid to the right
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ShiftRight();
    }

    void CreateGrid()
    {
        int cols = size.x;
        int rows = size.y;

        points = new PointMass[cols, rows];
        fixedPoints = new PointMass[cols, rows];
        var springList = new List<Spring>();

        // Create points
        for (int y = 0; y < rows; y++)
        for (int x = 0; x < cols; x++)
        {
            Vector3 pos = new Vector3(
                (originX + x) * spacing.x,
                (originY + y) * spacing.y,
                0f
            );

            points[x, y] = new PointMass(pos, 1f);
            fixedPoints[x, y] = new PointMass(pos, 0f);
        }

        // Create springs
        const float stiffness = 0.28f;
        const float damping = 0.06f;

        for (int y = 0; y < rows; y++)
        for (int x = 0; x < cols; x++)
        {
            // Edge points attached to fixed anchors
            if (x == 0 || y == 0 || x == cols - 1 || y == rows - 1)
                springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.1f, 0.1f));
            else if (x % 3 == 0 && y % 3 == 0)
                springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.002f, 0.02f));

            // Horizontal and vertical springs
            if (x > 0)
                springList.Add(new Spring(points[x - 1, y], points[x, y], stiffness, damping));
            if (y > 0)
                springList.Add(new Spring(points[x, y - 1], points[x, y], stiffness, damping));
        }

        springs = springList.ToArray();
    }

    // ============================
    // Sliding window: recycle points
    // ============================

    public void ShiftRight()
    {
        originX++; // camera moves right

        int cols = points.GetLength(0);
        int rows = points.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            // Save leftmost column to recycle
            PointMass recycled = points[0, y];

            // Shift references left
            for (int x = 0; x < cols - 1; x++)
                points[x, y] = points[x + 1, y];

            // Recycle leftmost column to the right
            points[cols - 1, y] = recycled;

            // Reset its position in world space
            ResetPoint(recycled, cols - 1, y);
        }
    }

    void ResetPoint(PointMass p, int localX, int localY)
    {
        int gx = originX + localX;
        int gy = originY + localY;

        p.pos = new Vector3(
            gx * spacing.x,
            gy * spacing.y,
            0f
        );

        p.velo = Vector3.zero;
    }

    // ============================
    // Forces
    // ============================

    public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
    {
        float r2 = radius * radius;

        foreach (var mass in points)
        {
            float dist2 = (mass.pos - position).sqrMagnitude;
            if (dist2 < r2)
            {
                mass.ApplyForce(force / (1f + Mathf.Sqrt(dist2)));
            }
        }
    }

    public void ApplyImplosiveForce(float force, Vector3 position, float radius)
    {
        float r2 = radius * radius;

        foreach (var mass in points)
        {
            Vector3 delta = position - mass.pos;
            float dist2 = delta.sqrMagnitude;

            if (dist2 < r2)
            {
                mass.ApplyForce(delta * force / (dist2 + 0.01f));
                mass.IncreaseDamping(0.6f);
            }
        }
    }

    public void ApplyExplosiveForce(float force, Vector3 position, float radius)
    {
        float r2 = radius * radius;

        foreach (var mass in points)
        {
            Vector3 delta = mass.pos - position;
            float dist2 = delta.sqrMagnitude;

            if (dist2 < r2)
            {
                mass.ApplyForce(delta * force / (dist2 + 0.01f));
                mass.IncreaseDamping(0.6f);
            }
        }
    }
}
