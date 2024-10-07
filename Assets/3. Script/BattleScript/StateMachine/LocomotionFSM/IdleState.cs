public class IdleState : IState
{
    Character owner;

    public IdleState(Character owner) { this.owner = owner; }

    public void OnEnter()
    {
        owner.locomotionData.locomotionState = ELocomotionState.Stop;
    }
    public void OnUpdate()
    {
        
    }

    public void OnExit()
    {
        
    }
}
