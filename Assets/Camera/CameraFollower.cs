using UnityEngine;
using System.Linq;
using Unity.Services.Lobbies.Models;

public class CameraFollower : MonoBehaviour
{
    GameObject ship;
    Camera mainCamera;

    Vector3 curCameraPosVelocity;
    float curCameraZoomVelocity;

    float startingZoom;

    [SerializeField] float ZOOM_PADDING = 2.0f;

    [SerializeField] float ZOOM_SMOOTH_MAX = 5.0f;
    [SerializeField] float ZOOM_SMOOTH_TIME = 3.0f;

    [SerializeField] float POS_SMOOTH_MAX = 5.0f;
    [SerializeField] float POS_SMOOTH_TIME = 3.0f;
    [SerializeField] [Range(0f, 1f)] float ENEMY_PLAYER_WEIGHT = 0f;

    [SerializeField] Vector2 cameraSize;

    Vector3 averagePosition;
    Vector3 targetPosition;

    void Start()
    {
        ship = GameObject.Find("Ship");

        mainCamera = Camera.main;

        startingZoom = mainCamera.orthographicSize;
    }

    void FixedUpdate()
    {
        // update camera sizes
        cameraSize = new Vector2(Camera.main.orthographicSize * Camera.main.aspect * 2, Camera.main.orthographicSize * 2);

        
        // calculate target position
        averagePosition = Vector3.zero;
        targetPosition = Vector3.zero;

        if (EnemyManager.chasingEnemies.Count > 0)
        {
            foreach (var enemy in EnemyManager.chasingEnemies)
                averagePosition += enemy.transform.position;

            averagePosition /= EnemyManager.chasingEnemies.Count;

            targetPosition = Vector3.Lerp(
                ship.transform.position,
                averagePosition,
                ENEMY_PLAYER_WEIGHT);
        }
        else
        {
            targetPosition = ship.transform.position;
        }

        targetPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);


        //apply smooth damp
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref curCameraPosVelocity,
            POS_SMOOTH_TIME,
            POS_SMOOTH_MAX);


        // calculate target zoom
        Vector2 farestEnemyPos = Vector2.zero;

        foreach (var enemy in EnemyManager.chasingEnemies)
        {
            if (Vector2.Distance(enemy.transform.position, ship.transform.position) > Vector2.Distance(farestEnemyPos, ship.transform.position))
                farestEnemyPos = enemy.transform.position;
        }

        float targetZoom = Mathf.Abs(targetPosition.x - farestEnemyPos.x) / 2 + ZOOM_PADDING;

        if (targetZoom < startingZoom)
            targetZoom = startingZoom;

        mainCamera.orthographicSize = Mathf.SmoothDamp(
            mainCamera.orthographicSize,
            targetZoom,
            ref curCameraZoomVelocity,
            ZOOM_SMOOTH_TIME,
            ZOOM_SMOOTH_MAX);
        

        //debug
        foreach (var enemy in EnemyManager.chasingEnemies.ToList())
        {
            Debug.DrawLine(averagePosition, enemy.transform.position, Color.red);
        }

        Debug.DrawLine(averagePosition, ship.transform.position, Color.green);
    }


    void OnDrawGizmos()
    {
        DebugDrawCircle(averagePosition, .3f);
        DebugDrawCircle(targetPosition, .3f);
    }

    void DebugDrawCircle(Vector2 pos, float radius)
    {
        int segments = 100;
        float angle = 0f;
        Vector3 lastPoint = pos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        for (int i = 0; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            Vector3 nextPoint = pos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(lastPoint, nextPoint, Color.softGreen);
            lastPoint = nextPoint;
        }
    }
}
