using UnityEngine;

namespace CombatSystem.Behavior
{
    [CreateAssetMenu(fileName = "Move To Target", menuName = "Scriptable Object/Behavior Actions/Move To Target")]
    public class MoveToTarget : SO_BehaviorAction
    {
        public ELocomotionState state;

        public override void Execute(BehaviorSets behavior)
        {
            CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
            if (enemy != null)
            {
                Character target = enemy.aiPerception.senseDectection.GetTarget();
                if (target != null)
                {
                    enemy.SetLocomotionState(state);
                    enemy.SetDestination(target.transform.position);
                    FinishExecute(behavior, true);

                    return;
                }
            }

            FinishExecute(behavior, false);
        }

        public override void Finish(BehaviorSets behavior)
        {

        }
    }
}