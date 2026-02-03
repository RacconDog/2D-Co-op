using UnityEngine;
using System.Collections.Generic;

public class GridMeshContinuous : MonoBehaviour
{
    public PointMass[,] points; 
    public int samplesPerSegment = 5; 
    public Material lineMaterial; 

    private List<Mesh> rowMeshes = new List<Mesh>();
    private List<Mesh> colMeshes = new List<Mesh>();
    private List<GameObject> rowObjects = new List<GameObject>();
    private List<GameObject> colObjects = new List<GameObject>();

    void Start()
    {
        points = this.GetComponent<Grid>().points;

        if (points == null) return;

        CreateRowMeshes();
        CreateColMeshes();
    }

    void Update()
    {
        UpdateRowMeshes();
        UpdateColMeshes();
    }

    public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    void CreateRowMeshes()
    {
        int cols = points.GetLength(0);
        int rows = points.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            GameObject go = new GameObject("RowMesh_" + y);
            go.transform.SetParent(transform);
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.material = lineMaterial;

            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            rowMeshes.Add(mesh);
            rowObjects.Add(go);
        }
    }

    void UpdateRowMeshes()
    {
        int cols = points.GetLength(0);
        int rows = points.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            Mesh mesh = rowMeshes[y];
            List<Vector3> verts = new List<Vector3>();

            for (int x = 0; x < cols - 1; x++)
            {
                Vector3 p0 = points[Mathf.Max(x - 1, 0), y].pos;
                Vector3 p1 = points[x, y].pos;
                Vector3 p2 = points[x + 1, y].pos;
                Vector3 p3 = points[Mathf.Min(x + 2, cols - 1), y].pos;

                for (int s = 0; s < samplesPerSegment; s++)
                {
                    float t = s / (float)(samplesPerSegment - 1);
                    verts.Add(CatmullRom(p0, p1, p2, p3, t));
                }
            }

            mesh.Clear();
            mesh.vertices = verts.ToArray();
            int[] indices = new int[verts.Count];
            for (int i = 0; i < verts.Count; i++) indices[i] = i;
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
            mesh.RecalculateBounds();
        }
    }

    void CreateColMeshes()
    {
        int cols = points.GetLength(0);
        int rows = points.GetLength(1);

        for (int x = 0; x < cols; x++)
        {
            GameObject go = new GameObject("ColMesh_" + x);
            go.transform.SetParent(transform);
            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            mr.material = lineMaterial;

            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            colMeshes.Add(mesh);
            colObjects.Add(go);
        }
    }

    void UpdateColMeshes()
    {
        int cols = points.GetLength(0);
        int rows = points.GetLength(1);

        for (int x = 0; x < cols; x++)
        {
            Mesh mesh = colMeshes[x];
            List<Vector3> verts = new List<Vector3>();

            for (int y = 0; y < rows - 1; y++)
            {
                Vector3 p0 = points[x, Mathf.Max(y - 1, 0)].pos;
                Vector3 p1 = points[x, y].pos;
                Vector3 p2 = points[x, y + 1].pos;
                Vector3 p3 = points[x, Mathf.Min(y + 2, rows - 1)].pos;

                for (int s = 0; s < samplesPerSegment; s++)
                {
                    float t = s / (float)(samplesPerSegment - 1);
                    verts.Add(CatmullRom(p0, p1, p2, p3, t));
                }
            }

            mesh.Clear();
            mesh.vertices = verts.ToArray();

            int[] indices = new int[verts.Count];
            for (int i = 0; i < verts.Count; i++) indices[i] = i;
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);

            Vector3[] normals = new Vector3[verts.Count];
            for (int i = 0; i < normals.Length; i++) normals[i] = Vector3.back; // Z = -1 for 2D
            mesh.normals = normals;

            Vector2[] uvs = new Vector2[verts.Count];
            for (int i = 0; i < uvs.Length; i++) uvs[i] = Vector2.zero;
            mesh.uv = uvs;

            mesh.RecalculateBounds();

        }
    }
}
