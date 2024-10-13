public class IdleState : IState
{
    CharacterActor owner;

    public IdleState(CharacterActor owner) { this.owner = owner; }

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
