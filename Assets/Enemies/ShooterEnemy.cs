using System.Xml;
using UnityEngine;

public class ShooterEnemy : AbstractEnemy
{   
    Transform shipTransform;
    [SerializeField] Transform shipSprite;
    bool shipInView;
    bool shipInRange;
    Animator animator;

    [SerializeField] EnemyData enemyData;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        shipTransform = GameObject.Find("Ship").transform;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, shipTransform.position) - shipSprite.localScale.x * 0.5f < enemyData.VIEW_DISTANCE)
        {
            shipInView = true;
        }
        else
        {
            shipInView = false;
        }

        if (Vector2.Distance(transform.position, shipTransform.position) > enemyData.RANGE - enemyData.RANGE_PADDING &&
            Vector2.Distance(transform.position, shipTransform.position) < enemyData.RANGE + enemyData.RANGE_PADDING)
        {
            shipInRange = true;
        }
        else
        {
            shipInRange = false;
        }

        animator.SetBool("ShipInView", shipInView);
        animator.SetBool("ShipInRange", shipInRange);

        DebugDrawCircle(shipTransform.position, enemyData.RANGE - enemyData.RANGE_PADDING, Color.softRed);
        DebugDrawCircle(shipTransform.position, enemyData.RANGE + enemyData.RANGE_PADDING, Color.softRed);
    }

    void DebugDrawCircle(Vector2 pos, float radius, Color color)
    {
        int segments = 100;
        float angle = 0f;
        Vector3 lastPoint = pos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

        for (int i = 0; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            Vector3 nextPoint = pos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(lastPoint, nextPoint, color);
            lastPoint = nextPoint;
        }
    }
}
