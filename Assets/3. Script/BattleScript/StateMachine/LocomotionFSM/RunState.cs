public class RunState : IState
{
    CharacterActor owner;

    public RunState(CharacterActor owner) { this.owner = owner; }

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
