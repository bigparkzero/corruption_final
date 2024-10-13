using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

public enum EStatType
{
    HP,
    MorphPoint,
    MorphGauge,
}

[Serializable]
public struct StatData
{
    public float maxHP;
    public float hp;

    [Space(10)]
    public int maxMorphPoint;
    public int morphPoint;

    [Space(10)]
    public float maxMorphGauge;
    public float morphGauge;

    [Space(10)]
    [Min(1)] public float atk;
}

public class StatsComponent : MonoBehaviour
{
    private CharacterBase owner;

    [SerializeField] StatData statData;
    public StatData GetStatData { get => statData; }

    [SerializeField] bool useInit;
    public bool isInvincible = false;
    public bool isAlive = true;
    public bool isRegenHP = false;
    [Range(0.1f, 10f)] public float hpRegenTick = 5f;
    [Min(0)] public float hpRegenAmount = 5f;

    public UnityAction<DamageInfo> OnDeadAction;
    public UnityAction<EStatType> OnResetAction;

    public Coroutine C_HpRegen;
    public UnityAction OnDead;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        owner = GetComponent<CharacterBase>();

        if (useInit)
        {
            statData.hp = statData.maxHP;
        }

        C_HpRegen = StartCoroutine(Regen());
    }

    IEnumerator Regen()
    {
        while (isAlive)
        {
            if (isRegenHP)
            {
                Healed(hpRegenAmount);
            }

            yield return new WaitForSeconds(hpRegenTick);
        }
    }

    public void Healed(float amount)
    {
        statData.hp = Mathf.Min(statData.hp + amount, statData.maxHP);
    }

    public void Damaged(float amount)
    {
        if (isInvincible) return;

        statData.hp = Mathf.Max(statData.hp - amount, 0);

        if (statData.hp <= 0)
        {
            owner.StopEveryCoroutines();
            isAlive = false;

            OnDead?.Invoke();
        }
    }
}
