using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Object/Behavior Actions/Attack")]
public class Attack : SO_BehaviorAction
{
    public override void Execute(BehaviorSets behavior)
    {
        CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
        if (enemy != null)
        {
            enemy.PlayCombo(behavior.currentAction.skill);
            FinishExecute(behavior, true);

            return;
        }

        FinishExecute(behavior, false);
    }

    public override void Finish(BehaviorSets behavior)
    {
            
    }
}