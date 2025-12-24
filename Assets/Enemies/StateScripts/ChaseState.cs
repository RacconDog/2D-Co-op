using UnityEngine;

public class ChaseState : AbstractEnemyState
{
    public ChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsChasing", true);
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsChasing", false);
    }
}
