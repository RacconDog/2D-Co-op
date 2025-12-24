using UnityEngine;

public abstract class AbstractEnemyState
{
    protected Enemy enemy;

    protected AbstractEnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

