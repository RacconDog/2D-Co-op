using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    [SerializeField] Vector2 size = new Vector2(20, 20);
    [SerializeField] Vector2 spacing = new Vector2(1, 1);

    public PointMass[,] points { get; private set; }
    PointMass[,] fixedPoints;

    Spring[] springs;
 
    void Awake()
    {
        CreateGrid(size, spacing);
    }

    void Update()
    {
        // update springs
        foreach (var spring in springs)
            spring.Update();

        // update points
        foreach (var p in points)
            p.UpdatePoint();
    }

    void CreateGrid(Vector2 size, Vector2 spacing)
    {
        var springList = new List<Spring>();

        int numColumns = (int)(size.x / spacing.x) + 1;
        int numRows = (int)(size.y / spacing.y) + 1;

        points = new PointMass[numColumns, numRows];
        fixedPoints = new PointMass[numColumns, numRows];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numColumns; x++)
            {
                Vector3 pos = new Vector3(x * spacing.x, y * spacing.y, 0);
                pos.x -= size.x / 2;
                pos.y -= size.y / 2;
                
                points[x, y] = new PointMass(pos, 1);
                fixedPoints[x, y] = new PointMass(pos, 0);
            }
        }


        for (int y = 0; y < numRows; y++)
        for (int x = 0; x < numColumns; x++)
        {
            if (x == 0 || y == 0 || x == numColumns - 1 || y == numRows - 1)
                springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.1f, 0.1f));
            else if (x % 3 == 0 && y % 3 == 0)
                springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.002f, 0.02f));

            const float stiffness = 0.28f;
            const float damping = 0.06f;

            if (x > 0)
                springList.Add(new Spring(points[x - 1, y], points[x, y], stiffness, damping));
            if (y > 0)
                springList.Add(new Spring(points[x, y - 1], points[x, y], stiffness, damping));
        }

        springs = springList.ToArray();
    }

    public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
    {
    	foreach (var mass in points)
    		if (Vector3.Distance(position, mass.pos) * Vector3.Distance(position, mass.pos) < radius * radius)
    			mass.ApplyForce(10 * force / (10 + Vector3.Distance(position, mass.pos)));
    }
    public void ApplyImplosiveForce(float force, Vector3 position, float radius)
    {
    	foreach (var mass in points)
    	{
    		float dist2 = Vector3.Distance(position, mass.pos) * Vector3.Distance(position, mass.pos);
    		if (dist2 < radius * radius)
    		{
    			mass.ApplyForce(10 * force * (position - mass.pos) / (100 + dist2));
    			mass.IncreaseDamping(0.6f);
    		}
    	}
    }
    public void ApplyExplosiveForce(float force, Vector3 position, float radius)
    {
    	foreach (var mass in points)
    	{
    		float dist2 = Vector3.Distance(position, mass.pos) * Vector3.Distance(position, mass.pos);
    		if (dist2 < radius * radius)
    		{
    			mass.ApplyForce(100 * force * (mass.pos - position) / (10000 + dist2));
    			mass.IncreaseDamping(0.6f);
    		}
    	}
    }
}
