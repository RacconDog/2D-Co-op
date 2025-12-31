using System.Xml;
using UnityEngine;

public class ShooterEnemy : AbstractEnemy
{   
    Transform shipTransform;
    bool shipInView;
    Animator animator;

    [SerializeField] EnemyData enemyData;

    void Start()
    {
        animator = GetComponent<Animator>();
        shipTransform = GameObject.Find("Ship").transform;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, shipTransform.position) < enemyData.VIEW_DISTANCE)
        {
            shipInView = true;
        }
        else
        {
            shipInView = false;
        }

        animator.SetBool("ShipInView", shipInView);
        DebugDrawCircle(transform.position, enemyData.VIEW_DISTANCE);
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
            Debug.DrawLine(lastPoint, nextPoint, Color.red);
            lastPoint = nextPoint;
        }
    }
}
