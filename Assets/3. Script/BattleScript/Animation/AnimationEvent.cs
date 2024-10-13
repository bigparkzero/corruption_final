using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Animation Event Callback Script
public class AnimationEvent : MonoBehaviour
{
    CharacterActor owner;

    private void Start()
    {
        owner = GetComponent<CharacterActor>();
    }

    public void Anim_OnSkillBegin()
    {
        if (owner.attackComponent == null) return;

        owner.attackComponent.OnSkillBegin();
    }

    public void Anim_OnSkillEnd(int skillEnd)
    {
        if (owner.attackComponent == null) return;

        owner.attackComponent.OnSkillEnd(skillEnd);
    }

    public void Anim_OnAttack()
    {
        owner.hitReactionComponent.combatData.combatStanceType = ECombatStanceType.AttackStance;
        owner.hitReactionComponent.combatData.combatType = ECombatType.Attack;

        if (owner.attackComponent != null)
        {
            if (!string.IsNullOrEmpty(owner.attackComponent.CurrentSkillData.skillName) &&
                owner.attackComponent.CurrentSkillData.hitReactionDatas.Count > 0)
            {
                owner.hitReactionComponent.SetHitReaction(owner.attackComponent.CurrentSkillData.hitReactionDatas[owner.attackComponent.CurrentSkillIndex].hitReactionData);
            }
        }
        owner.hitReactionComponent.SpawnAttackFX();
        owner.hitTraceComponent.OnTraceBegin();

        CharacterEnemy enemy = owner as CharacterEnemy;
        if (enemy != null)
        {
            Character target = enemy.aiPerception.senseDectection.GetTarget();
            if (target != null)
            {
                owner.DoLookOpposite(target.transform.position, 0.5f);
            }
        }
    }

    public void Anim_OffAttack()
    {
        owner.hitReactionComponent.combatData.combatStanceType = ECombatStanceType.DefaultStance;
        owner.hitReactionComponent.combatData.combatType = ECombatType.None;
        owner.hitTraceComponent.OnTraceEnd();
    }

    public void Anim_OnDodge()
    {
        owner.hitReactionComponent.combatData.combatType = ECombatType.Dodge;
        owner.hitTraceComponent.OnTraceEnd();
    }

    public void Anim_OffDodge()
    {
        owner.hitReactionComponent.combatData.combatType = ECombatType.None;
    }

    public void Anim_OnFreeFlow()
    {
        if (owner.freeFlowComponent != null)
        {
            owner.freeFlowComponent.OnFreeflow();
        }
    }

    public void Anim_OnHit()
    {
        owner.hitReactionComponent.combatData.combatType = ECombatType.HitReaction;
        owner.hitTraceComponent.OnTraceEnd();

        if (owner.attackComponent != null)
        {
            owner.attackComponent.ResetSkill();
        }
    }

    public void Anim_OffHit()
    {
        owner.hitReactionComponent.combatData.combatType = ECombatType.None;
    }

    public void Anim_GravityEnable(float jumpForce = 0.0f)
    {
        owner.locomotionData.useGravity = true;
        //owner.locomotionData.jumpForce = jumpForce;
    }

    public void Anim_GravityDisable(float jumpForce = 0.0f)
    {
        owner.locomotionData.useGravity = false;
        //owner.locomotionData.jumpForce = jumpForce;
    }
}
