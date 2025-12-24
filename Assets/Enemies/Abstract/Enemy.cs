using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public Animator animator;

    public EnemyStateMachine StateMachine { get; private set; }

    public IdleState idleState;
    public ChaseState chaseState;

    [SerializeField] private string currentStateDebug;

    protected virtual void Awake()
    {
        StateMachine = new EnemyStateMachine();

        idleState = new IdleState(this);
        StateMachine.ChangeState(idleState);
    }

    protected virtual void Update()
    {
        StateMachine.CurrentState?.Update();

        // update debug var
        currentStateDebug = StateMachine.CurrentState != null
            ? StateMachine.CurrentState.GetType().Name
            : "None";
    }
}

