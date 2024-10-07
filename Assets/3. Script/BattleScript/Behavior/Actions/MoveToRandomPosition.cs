using UnityEngine;

namespace CombatSystem.Behavior
{
    [CreateAssetMenu(fileName = "Move To Random Position", menuName = "Scriptable Object/Behavior Actions/Move To Random Position")]
    public class MoveToRandomPosition : SO_BehaviorAction
    {
        public ELocomotionState state;

        public override void Execute(BehaviorSets behavior)
        {
            CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
            if (enemy != null)
            {
                enemy.SetLocomotionState(state);
                enemy.SetRandomMove(enemy.transform.position);
                FinishExecute(behavior, true);

                return;
            }

            FinishExecute(behavior, false);
        }

        public override void Finish(BehaviorSets behavior)
        {
            CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
            if (enemy != null)
            {
                enemy.StopRandomMove();
            }
        }
    }
}