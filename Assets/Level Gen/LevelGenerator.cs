using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] GameObject planetePrefab;
    [SerializeField] float levelRadius = 50f;

    enum PlanetSize { Small, Large }


    [Header("Large Planets")]
    [SerializeField] int largePlanetNum = 3;

    [SerializeField] float LargePlanetSeperation = 3;
    [SerializeField] float largePlanetSize = 20f;
    [SerializeField] float largePlanetSizeVariation = 5f;

    List<Transform> largePlanetList = new List<Transform>();

    [Header("Small Planets")]
    [SerializeField] int smallPlanetNum = 20;

    [SerializeField] float smallPlanetSeparation = 3;
    [SerializeField] float smallPlanetSize = 5f;
    [SerializeField] float smallPlanetSizeVariation = 2f;

    List<Transform> smallPlanetList = new List<Transform>();

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        foreach (Transform planet in largePlanetList.Concat(smallPlanetList))
        {
            Destroy(planet.gameObject);
        }
        
        largePlanetList.Clear();
        smallPlanetList.Clear();

        //large planets
        for (int i = 0; i < largePlanetNum; i++)
        {
            Vector2 pos = RandomPointInCircle();

            for (int j = 0; j < 1000; j++)
            {
                pos = RandomPointInCircle();

                if (IsPlanetSeparated(pos, false))
                {
                    break;
                }
            }

            GameObject planet = Instantiate(planetePrefab, (Vector3)pos + transform.position, Quaternion.identity);
            planet.transform.parent = transform;
            planet.transform.localScale = Vector3.one * (largePlanetSize + Random.Range(-largePlanetSizeVariation, largePlanetSizeVariation));
            largePlanetList.Add(planet.transform);
        }
    
        //small planets
        for (int i = 0; i < smallPlanetNum; i++)
        {
            Vector2 pos = RandomPointInCircle();

            for (int j = 0; j < 1000; j++)
            {
                pos = RandomPointInCircle();

                if (IsPlanetSeparated(pos, true))
                {
                    break;
                }
            }

            GameObject planet = Instantiate(planetePrefab, (Vector3)pos + transform.position, Quaternion.identity);
            planet.transform.parent = transform;
            planet.transform.localScale = Vector3.one * (smallPlanetSize + Random.Range(-smallPlanetSizeVariation, smallPlanetSizeVariation));
            smallPlanetList.Add(planet.transform);
        }
    }

    Vector2 RandomPointInCircle()
    {
        Vector2 r = Random.insideUnitCircle * levelRadius;
        // print(r);
        return r;
    }

    void Update()
    {
        Debug.DrawLine(Vector2.zero, Vector2.right * levelRadius, Color.red);

        foreach (Transform child in transform)
        {
            DrawDebugCircle(child.position, LargePlanetSeperation);
            DrawDebugCircle(child.position, .1f);
        }

        DrawDebugCircle(Vector2.zero, levelRadius);

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    void DrawDebugCircle(Vector3 center, float radius, int segments = 100)
    {
        float step = 2f * Mathf.PI / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), Mathf.Sin(0f), 0f) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = step * i;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            Debug.DrawLine(prevPoint, newPoint, Color.green, 0f);
            prevPoint = newPoint;
        }
    }

    bool IsPlanetSeparated(Vector2 position, bool isSmall)
    {
        if (largePlanetList.Count == 0)
        {
            return true;
        }

        if (isSmall == false)
        {
            int planetsChecked = 0;
            foreach (Transform planet in largePlanetList)
            {
                if (Vector2.Distance(position, (Vector2)planet.position) > LargePlanetSeperation * 2)
                {
                    planetsChecked++;
                }
            }

            return planetsChecked == largePlanetList.Count;
        }

        if (isSmall == true)
        {
            int planetsChecked = 0;
            foreach (Transform planet in largePlanetList)
            {
                if (Vector2.Distance(position, (Vector2)planet.position) > LargePlanetSeperation + smallPlanetSeparation)
                {
                    planetsChecked++;
                }
            }

            foreach (Transform planet in smallPlanetList)
            {
                if (Vector2.Distance(position, (Vector2)planet.position) > smallPlanetSeparation * 2)
                {
                    planetsChecked++;
                }
            }

            return planetsChecked == largePlanetList.Count + smallPlanetList.Count;
        }

        return false;
    }
}
