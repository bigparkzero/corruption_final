using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleStand", menuName = "Scriptable Object/Behavior Actions/IdleStand")]
public class IdleStand : SO_BehaviorAction
{
    public ELocomotionState state;

    public override void Execute(BehaviorSets behavior)
    {
        CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
        if (enemy != null)
        {
            Character target = enemy.aiPerception.senseDectection.GetTarget();
            if (target == null)
            {
                enemy.SetLocomotionState(state);
                enemy.SetDestination(enemy.transform.position);
                FinishExecute(behavior, true);
            }
        }

        FinishExecute(behavior, false);
    }

    public override void Finish(BehaviorSets behavior)
    {

    }
}
