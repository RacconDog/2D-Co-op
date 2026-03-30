using UnityEngine;
using System.Collections.Generic;

public class GridMeshContinuous : MonoBehaviour
{
    public Material lineMaterial;
    public float lineThickness = 0.05f; // adjustable thickness

    PointMass[,] points;
    int cols, rows;

    Mesh rowMesh;
    Mesh colMesh;

    Bounds bounds;

    void Start()
    {
        points = GetComponent<Grid>().points;
        if (points == null) return;

        cols = points.GetLength(0);
        rows = points.GetLength(1);

        CreateMeshes();
    }

    void CreateMeshes()
    {
        // ---- ROW MESH ----
        {
            GameObject go = new GameObject("Rows");
            go.transform.SetParent(transform);

            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.material = lineMaterial;

            rowMesh = new Mesh();
            rowMesh.MarkDynamic();
            mf.mesh = rowMesh;
        }

        // ---- COLUMN MESH ----
        {
            GameObject go = new GameObject("Cols");
            go.transform.SetParent(transform);

            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.material = lineMaterial;

            colMesh = new Mesh();
            colMesh.MarkDynamic();
            mf.mesh = colMesh;
        }

        bounds = new Bounds(Vector3.zero, Vector3.one * 1000f);
    }

    void Update()
    {
        UpdateRowMesh(lineThickness);
        UpdateColMesh(lineThickness);
    }

    // ---- Helper to create a quad for a thick line ----
    Vector3[] CreateQuad(Vector3 start, Vector3 end, float width)
    {
        Vector3 dir = (end - start).normalized;
        Vector3 perp = Vector3.Cross(dir, Vector3.forward) * width * 0.5f; // XY plane
        return new Vector3[]
        {
            start - perp,
            start + perp,
            end + perp,
            end - perp
        };
    }

    void UpdateRowMesh(float thickness)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int idx = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols - 1; x++)
            {
                Vector3 start = points[x, y].pos;
                Vector3 end = points[x + 1, y].pos;

                Vector3[] quad = CreateQuad(start, end, thickness);
                verts.AddRange(quad);

                tris.Add(idx);
                tris.Add(idx + 1);
                tris.Add(idx + 2);
                tris.Add(idx);
                tris.Add(idx + 2);
                tris.Add(idx + 3);

                idx += 4;
            }
        }

        rowMesh.Clear();
        rowMesh.SetVertices(verts);
        rowMesh.SetTriangles(tris, 0);
        rowMesh.bounds = bounds;
    }

    void UpdateColMesh(float thickness)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int idx = 0;

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows - 1; y++)
            {
                Vector3 start = points[x, y].pos;
                Vector3 end = points[x, y + 1].pos;

                Vector3[] quad = CreateQuad(start, end, thickness);
                verts.AddRange(quad);

                tris.Add(idx);
                tris.Add(idx + 1);
                tris.Add(idx + 2);
                tris.Add(idx);
                tris.Add(idx + 2);
                tris.Add(idx + 3);

                idx += 4;
            }
        }

        colMesh.Clear();
        colMesh.SetVertices(verts);
        colMesh.SetTriangles(tris, 0);
        colMesh.bounds = bounds;
    }
}
