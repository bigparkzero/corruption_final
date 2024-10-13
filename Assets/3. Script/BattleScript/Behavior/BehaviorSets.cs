using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBehaviorTask
{
    None = 0,

    Inactive,
    Running,
    Waiting,
    Succeeded,
    Failed,
    Aborted
}

[Serializable]
public struct BehaviorData
{
    public List<Condition> conditions;
    public List<Action> actions;
}

public class BehaviorSets : MonoBehaviour
{
    private CharacterActor owner;
    public CharacterActor Owner { get => owner; }

    [Header("[Behavior Sets]")]
    public EBehaviorTask behaviorTask;
    public List<BehaviorData> datas;
    public float intervalUpdate = 0.1f;
    public Action currentAction;
    public Action prevAction;

    [Header("[Coroutine]")]
    public Coroutine C_Behavior;
    public Coroutine C_Cooldown;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        owner = GetComponent<CharacterActor>();

        StartBehavior();
    }

    private IEnumerator Behavior()
    {
        behaviorTask = EBehaviorTask.Running;
        while (true)
        {
            if (!owner.statsComponent.isAlive) yield break;
            EvaluateAndExecuteBehavior();
            if (currentAction.behaviorAction != null && currentAction.cooldown)
            {
                SetCooldown(UnityEngine.Random.Range(currentAction.minCooldown, currentAction.maxCooldown));
            }
            yield return new WaitWhile(() => behaviorTask == EBehaviorTask.Waiting);
            yield return new WaitForSeconds(intervalUpdate);
        }
    }

    private IEnumerator Cooldown(float cooldownTime)
    {
        behaviorTask = EBehaviorTask.Waiting;
        yield return new WaitForSeconds(cooldownTime);
        behaviorTask = EBehaviorTask.Succeeded;
    }

    #region Condition

    #region Comparison

    private bool EvaluateCheckBool(CheckValue<bool> checkBool)
    {
        bool variableValue = GetBoolValue(checkBool.variableType);
        switch (checkBool.comparison)
        {
            case EComparison.EqualTo:
                return variableValue == checkBool.value;

            default:
                return false;
        }
    }

    private bool EvaluateCheckInt(CheckValue<int> checkInt)
    {
        int variableValue = GetIntValue(checkInt.variableType);
        switch (checkInt.comparison)
        {
            case EComparison.EqualTo:
                return variableValue == checkInt.value;

            case EComparison.GreaterThan:
                return variableValue > checkInt.value;

            case EComparison.LessThan:
                return variableValue < checkInt.value;

            default:
                return false;
        }
    }

    private bool EvaluateCheckFloat(CheckValue<float> checkFloat)
    {
        float variableValue = GetFloatValue(checkFloat.variableType);
        switch (checkFloat.comparison)
        {
            case EComparison.EqualTo:
                return Mathf.Approximately(variableValue, checkFloat.value);

            case EComparison.GreaterThan:
                return variableValue > checkFloat.value;

            case EComparison.LessThan:
                return variableValue < checkFloat.value;

            default:
                return false;
        }
    }

    private bool EvaluateCondition(Condition condition)
    {
        foreach (var checkBool in condition.checkBool)
        {
            if (!EvaluateCheckBool(checkBool)) return false;
        }

        foreach (var checkInt in condition.checkInt)
        {
            if (!EvaluateCheckInt(checkInt)) return false;
        }

        foreach (var checkFloat in condition.checkFloat)
        {
            if (!EvaluateCheckFloat(checkFloat)) return false;
        }

        return true;
    }

    #endregion

    #region Variable

    private bool GetBoolValue(EVariableType variableType)
    {
        switch (variableType)
        {
            case EVariableType.Target:
                CharacterEnemy enemy = owner as CharacterEnemy;
                if (enemy != null)
                {
                    return enemy.aiPerception.senseDectection.GetTarget() != null;
                }
                return false;

            case EVariableType.Attacking:
                return owner.hitReactionComponent.combatData.combatType == ECombatType.Attack;

            default:
                return false;
        }
    }

    private int GetIntValue(EVariableType variableType)
    {
        switch (variableType)
        {
            default:
                return 0;
        }
    }

    private float GetFloatValue(EVariableType variableType)
    {
        switch (variableType)
        {
            case EVariableType.Distance:
                {
                    CharacterEnemy enemy = owner as CharacterEnemy;
                    if (enemy != null)
                    {
                        return enemy.aiPerception.senseDectection.GetDistanceToTarget();
                    }
                    return 0.0f;
                }

            case EVariableType.AngleToTarget:
                {
                    CharacterEnemy enemy = owner as CharacterEnemy;
                    if (enemy != null)
                    {
                        Vector3 direction = Util.GetDirection(enemy.transform.position, enemy.navMeshAgent.steeringTarget);
                        float angle = Vector3.Angle(enemy.transform.forward, direction);
                        return angle;
                    }
                    return 0.0f;
                }

            default:
                return 0.0f;
        }
    }

    #endregion

    #endregion

    public void EvaluateAndExecuteBehavior()
    {
        List<Action> suffleActions = new List<Action>();
        float totalScore = 0.0f;

        foreach (var data in datas)
        {
            bool allConditionsMatching = true;
            foreach (var condition in data.conditions)
            {
                if (!EvaluateCondition(condition))
                {
                    allConditionsMatching = false;
                    break;
                }
            }

            if (allConditionsMatching)
            {
                foreach (var action in data.actions)
                {
                    suffleActions.Add(action);
                    totalScore += action.score;
                }
            }
        }

        if (suffleActions.Count > 0)
        {
            float randomValue = UnityEngine.Random.Range(0, totalScore);
            float cumulativeScore = 0.0f;

            foreach (var action in suffleActions)
            {
                cumulativeScore += action.score;
                if (randomValue <= cumulativeScore)
                {
                    Execute(action);
                    break;
                }
            }
        }
    }

    private void Execute(Action action)
    {
        if (action.IsEmpty())
        {
            Debug.LogError("Behavior Action is null");
            return;
        }
        //Debug.Log($"Behavior Action Name : {action.behaviorAction.name}");

        if (currentAction.behaviorAction != null)
        {
            prevAction = currentAction;
            prevAction.behaviorAction.Clone().Finish(this);
        }
        currentAction = action;
        currentAction.behaviorAction.Clone().Execute(this);
    }

    public void Execute(SO_BehaviorAction action)
    {
        if (action == null) return;

        action.Execute(this);
    }

    public void StartBehavior()
    {
        StopBehavior();
        C_Behavior = StartCoroutine(Behavior());
    }

    public void StopBehavior()
    {
        if (C_Behavior != null)
        {
            StopCoroutine(C_Behavior);
            behaviorTask = EBehaviorTask.Inactive;
        }
    }

    public void ResetBehavior()
    {
        datas.Clear();
    }

    public void SetCooldown(float cooldownTime)
    {
        if (C_Cooldown != null) StopCoroutine(C_Cooldown);
        C_Cooldown = StartCoroutine(Cooldown(cooldownTime));
    }
}
