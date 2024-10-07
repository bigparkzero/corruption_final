using UnityEngine;

[CreateAssetMenu(fileName = "Jump", menuName = "Scriptable Object/Behavior Actions/Jump")]
public class Jump : SO_BehaviorAction
{
    public override void Execute(BehaviorSets behavior)
    {
        behavior.Owner.DoJump();
        FinishExecute(behavior, true);
    }

    public override void Finish(BehaviorSets behavior)
    {
        FinishExecute(behavior, true);
    }
}