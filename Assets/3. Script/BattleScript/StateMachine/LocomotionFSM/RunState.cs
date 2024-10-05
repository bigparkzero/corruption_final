public class RunState : IState
{
    Character owner;

    public RunState(Character owner) { this.owner = owner; }

    public void OnEnter()
    {
        owner.locomotionData.locomotionState = ELocomotionState.Sprint;
        owner.locomotionData.isSprint = true;
    }
    public void OnUpdate()
    {

    }

    public void OnExit()
    {
        owner.locomotionData.isSprint = false;
    }
}
