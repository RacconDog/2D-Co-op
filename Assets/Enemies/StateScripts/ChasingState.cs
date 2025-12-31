using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChasingState : AbstractEnemyState
{    
    Transform shipTransform;
    GameObject THIS;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shipTransform = GameObject.Find("Ship").transform;
        THIS = animator.gameObject;

        EnemyManager.AddEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 dir = shipTransform.position;
        dir.Normalize();

        THIS.transform.position = Vector2.MoveTowards(THIS.transform.position, shipTransform.position, Time.deltaTime * enemyData.SPEED);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyManager.RemoveEnemy(animator.gameObject, EnemyManager.EnemyState.Chasing);
    }
}
