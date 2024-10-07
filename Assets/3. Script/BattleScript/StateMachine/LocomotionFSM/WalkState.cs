public class WalkState : IState
{
    Character owner;

    public WalkState(Character owner) { this.owner = owner; }

    public void OnEnter()
    {
        owner.locomotionData.locomotionState = ELocomotionState.Walk;
    }
    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
