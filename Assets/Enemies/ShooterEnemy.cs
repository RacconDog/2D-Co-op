using UnityEngine;

public class ShooterEnemy : Enemy
{

    override protected void Awake()
    {
        base.Awake();
        
        chaseState = new ChaseState(this);
    }

    override protected void Update()
    {
        base.Update();
        
    }
}
