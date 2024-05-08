using Shared;


public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }

    public PlayerStateMachine()
    {
        CurrentState = PlayerState.Idle;
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentState = newState;
    }
}
