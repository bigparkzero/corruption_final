using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Confront", menuName = "Scriptable Object/Behavior Actions/Confront")]
public class Confront : SO_BehaviorAction
{
    public ELocomotionState state;

    public override void Execute(BehaviorSets behavior)
    {
        CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
        if (enemy != null)
        {
            CharacterActor target = enemy.aiPerception.senseDectection.GetTarget();
            if (target != null)
            {
                if (Util.GetDistance(enemy, target) <= enemy.navMeshAgent.stoppingDistance)
                {
                    enemy.SetLocomotionState(state);
                    enemy.SetDestination(target.transform.position);
                    FinishExecute(behavior, true);
                    return;
                }

            }
        }

        FinishExecute(behavior, false);
    }

    public override void Finish(BehaviorSets behavior)
    {

    }
}
