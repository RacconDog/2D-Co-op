using System.Diagnostics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChasingState : AbstractEnemyState
{    
    Transform shipTransform;
    GameObject THIS;

    Vector2 smoothDampVelocity;

    Vector2 targetPosition;
    Vector3 startPosition;

    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shipTransform = GameObject.Find("Ship").transform;
        THIS = animator.gameObject;

        EnemyManager.AddEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);

        startPosition = THIS.transform.position;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //rotate to face 
        THIS.transform.right = -(shipTransform.position - THIS.transform.position);

        //move towards ship
        targetPosition = (startPosition - shipTransform.position).normalized * enemyData.RANGE;
        
        THIS.transform.position = Vector2.SmoothDamp(
            THIS.transform.position,
            targetPosition,
            ref smoothDampVelocity, 
            enemyData.SPEED_ACCEL, 
            enemyData.SPEED_MAX);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyManager.RemoveEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);
    }
}
