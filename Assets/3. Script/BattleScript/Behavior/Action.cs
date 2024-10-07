using System;
using UnityEngine;

[Serializable]
public struct Action
{
    [Header("[Action]")]
    public SO_BehaviorAction behaviorAction;
    public SO_Skill skill;
    public float score;

    [Header("[Cooldown]")]
    public bool cooldown;
    public float minCooldown;
    public float maxCooldown;

    public bool IsEmpty()
    {
        return (behaviorAction == null && skill == null);
    }
}