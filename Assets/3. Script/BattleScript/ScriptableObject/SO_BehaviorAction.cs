using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_BehaviorAction : ScriptableObject, IBehaviorAction
{
    public abstract void Execute(BehaviorSets behavior);

    public abstract void Finish(BehaviorSets behavior);

    public void FinishExecute(BehaviorSets behavior, bool isSuccess)
    {
        if (behavior == null) return;

        behavior.behaviorTask = isSuccess ? EBehaviorTask.Succeeded : EBehaviorTask.Failed;
    }

    public SO_BehaviorAction Clone()
    {
        SO_BehaviorAction action = Instantiate(this);

        return action;
    }
}
