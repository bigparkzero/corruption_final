public class LocomotionFSM : StateMachine
{
    public IdleState idleState;
    public WalkState walkState;
    public RunState runState;

    public LocomotionFSM(Character owner)
    {
        idleState = new IdleState(owner);
        walkState = new WalkState(owner);
        runState = new RunState(owner);
    }
}
