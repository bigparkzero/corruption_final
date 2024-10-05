using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [Header("[Skill]")]
    public Character owner;
    public string skillName;
    public float cooldownTime;
    public float mana;
    private float T_Skill;

    public abstract void Active(Character target = null);

    public abstract void Inactive();

    public bool IsSkillReady()
    {
        return Time.time >= T_Skill;
    }

    public void StartCooldown()
    {
        T_Skill = Time.time + cooldownTime;
    }
}
