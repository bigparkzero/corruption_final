using UnityEngine;

[CreateAssetMenu(fileName = "Run Away", menuName = "Scriptable Object/Behavior Actions/Run Away")]

public class RunAway : SO_BehaviorAction
{
    public enum EDirectionType
    {
        Linear,
        Circle,
    }

    public ELocomotionState state;
    public EDirectionType type;
    public float moveDistance = 2.0f;

    public override void Execute(BehaviorSets behavior)
    {
        CharacterEnemy enemy = behavior.Owner as CharacterEnemy;
        if (enemy != null)
        {
            enemy.SetLocomotionState(state);

            Character target = enemy.aiPerception.senseDectection.GetTarget();
            if (target != null)
            {
                Vector3 direction = Util.GetDirection(target.transform.position, enemy.transform.position);

                switch (type)
                {
                    case EDirectionType.Linear:
                        enemy.SetDestination(enemy.transform.position + (direction * moveDistance));
                        break;

                    case EDirectionType.Circle:
                        enemy.SetDestination(direction * moveDistance);
                        break;
                }
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