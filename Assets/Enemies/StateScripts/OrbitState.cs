using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrbitState : AbstractEnemyState
{
    Transform shipTransform;
    GameObject thisGO;
    Rigidbody2D rb;

    Vector2 smoothDampVelocity;
    Vector2 targetPosition;

    bool isDrifting;
    bool hasDrifted;
    int randomDir = 1;

    float driftAngle = 0;
    Vector2 driftVector = Vector2.zero;

    float curTime = 0;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyManager.AddEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);

        isDrifting = true;
        hasDrifted = false;

        animator.SetBool("IsDrifting", isDrifting);

        shipTransform = GameObject.Find("Ship").transform;
        thisGO = animator.gameObject;
        rb = thisGO.GetComponent<Rigidbody2D>();

        curTime = 1f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        curTime -= Time.deltaTime;

        animator.SetBool("IsDrifting", isDrifting);

        Debug.DrawRay(thisGO.transform.position, driftVector, Color.cyan);

        thisGO.transform.right = -(shipTransform.position - thisGO.transform.position);


        if (!hasDrifted)
        {
            hasDrifted = true;

            randomDir = Random.Range(0, 2) == 0 ? -1 : 1;

            float driftAngle = Mathf.Atan2(
                shipTransform.position.y - thisGO.transform.position.y, 
                shipTransform.position.x - thisGO.transform.position.x);

            driftAngle += thisGO.transform.rotation.eulerAngles.z;
            driftAngle += randomDir * enemyData.DRIFT_OFFSET_ANGLE;

            driftAngle -= Random.Range(-enemyData.DRIFT_OFFSET_ANGLE_PADDING, enemyData.DRIFT_OFFSET_ANGLE_PADDING) / 2;

            driftVector = new Vector2(
                Mathf.Cos(driftAngle * Mathf.Deg2Rad), 
                Mathf.Sin(driftAngle * Mathf.Deg2Rad))
                * enemyData.DRIFT_FORCE;


            rb.AddForce(driftVector);
        }

        if (rb.linearVelocity.magnitude < .1f && curTime <= 0)
        {
            isDrifting = false;
        }


        // THIS.transform.position = Vector2.SmoothDamp(
        //     THIS.transform.position,
        //     targetPosition,
        //     ref smoothDampVelocity, 
        //     enemyData.SPEED_ACCEL, 
        //     enemyData.SPEED_MAX);

        //initate drift force

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyManager.RemoveEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);
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
