public interface IBehaviorAction
{
    public abstract void Execute(BehaviorSets behavior);

    public abstract void Finish(BehaviorSets behavior);

    public void FinishExecute(BehaviorSets behavior, bool isSuccess);
}