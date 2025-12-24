using UnityEngine;

public class IdleState : AbstractEnemyState
{
    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsIdle", true);
    }

    public override void Update()
    {

    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsIdle", false);
    }
}
