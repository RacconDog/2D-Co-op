using System.Diagnostics;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChasingState : AbstractEnemyState
{    
    Transform shipTransform;
    GameObject thisGO;

    Vector2 smoothDampVelocity;

    Vector2 targetPosition;
    Vector3 startPosition;

    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shipTransform = GameObject.Find("Ship").transform;
        thisGO = animator.gameObject;

        EnemyManager.AddEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);

        startPosition = thisGO.transform.position;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //rotate to face 
        thisGO.transform.right = -(shipTransform.position - thisGO.transform.position);

        //move towards ship
        targetPosition = (startPosition - shipTransform.position).normalized * enemyData.RANGE;
        
        thisGO.transform.position = Vector2.SmoothDamp(
            thisGO.transform.position,
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
