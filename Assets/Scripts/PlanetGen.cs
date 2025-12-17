using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetGen : MonoBehaviour
{
    public int points = 64;
    public float size = 2f;
    public float noiseScale = 1f;
    public float noiseStrength = 0.5f;
    public float seed = 0f;

    public void GeneratePlanet()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[points + 1];
        int[] triangles = new int[points * 3];

        // center point
        vertices[0] = Vector3.zero;

        for (int i = 0; i < points; i++)
        {
            float angle = (Mathf.PI * 2f * i) / points;
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);

            // add some "blobbiness"
            //we need to add noise scale so that points don't go negative
            float noise = Mathf.PerlinNoise(x * noiseScale + noiseScale + seed, y * noiseScale + noiseScale + seed);
            float r = size * (noise + noiseStrength);

            // x += Mathf.Cos(angle + 2 * Time.time) * 0.1f;
            // y += Mathf.Sin(angle + 2 * Time.time) * 0.1f;

            vertices[i + 1] = new Vector3(x * r, y * r, 0f);
        }

        for (int i = 0; i < points; i++)
        {
            int next = (i + 1) % points;
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = next + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void RandomizeSeed()
    {
        seed = Random.Range(-10000f, 10000f);
    }

    void Start()
    {
        RandomizeSeed();
        GeneratePlanet();
    }

    void Update()
    {
        // GeneratePlanet();
    }
}
