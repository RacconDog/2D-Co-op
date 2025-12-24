public class EnemyStateMachine
{
    public AbstractEnemyState CurrentState { get; private set; }

    public void ChangeState(AbstractEnemyState newState)
    {
        // Exit current state if it exists
        CurrentState?.Exit();

        // Set the new state
        CurrentState = newState;

        // Enter the new state
        CurrentState.Enter();
    }
}
