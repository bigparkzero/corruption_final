public class WalkState : IState
{
    CharacterActor owner;

    public WalkState(CharacterActor owner) { this.owner = owner; }

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
