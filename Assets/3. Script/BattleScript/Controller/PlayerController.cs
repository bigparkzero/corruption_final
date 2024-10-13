using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterActor owner;

    CursorLockMode cursorLockMode = CursorLockMode.Locked;
    bool cursorVisible = false;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Sprint();

        Jump();

        Roll();
        Dash();
        BasicAttack();
        SpecialSkill();
    }

    void Init()
    {
        owner = GetComponent<CharacterActor>();

        Cursor.lockState = cursorLockMode;
        Cursor.visible = cursorVisible;
    }

    void Sprint()
    {
        if (owner.attackComponent.skillState == ESkillState.Playing) return;

        owner.locomotionData.isSprint = PlayerInputManager.Instance.IsKeyStay(EKeyName.Sprint);
    }

    void Jump()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction ||
            owner.hitReactionComponent.combatData.combatType == ECombatType.Dodge ||
            owner.hitReactionComponent.combatData.combatType == ECombatType.Attack) return;

        if (owner.wallRideComponent?.wallRideState == EWallRideState.Active) return;

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Jump))
        {
            if (owner.locomotionData.isGrounded)
            {
                owner.DoJump();
            }
            owner.attackComponent.ResetSkill();
        }
    }

    void BasicAttack()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction) return;

        if (owner.wallRideComponent?.wallRideState == EWallRideState.Active) return;

        if (owner.hitReactionComponent.combatData.combatType == ECombatType.Dodge) return;

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Attack))
        {
            owner.attackComponent.SetInput(EKeyName.Attack);
        }
    }

    void SpecialSkill()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction) return;

        if (owner.wallRideComponent?.wallRideState == EWallRideState.Active) return;

        if (owner.hitReactionComponent.combatData.combatType == ECombatType.Dodge) return;

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Skill1))
        {
            owner.attackComponent.SetInput(EKeyName.Skill1);
        }
        else if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Skill2))
        {
            owner.attackComponent.SetInput(EKeyName.Skill2);
        }
        else if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Skill3))
        {
            owner.attackComponent.SetInput(EKeyName.Skill3);
        }
        else if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Skill4))
        {
            owner.attackComponent.SetInput(EKeyName.Skill4);
        }
    }

    void Roll()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction ||
            !owner.locomotionData.isGrounded ||
            owner.attackComponent.skillState == ESkillState.Playing) return;

        if (owner.wallRideComponent?.wallRideState == EWallRideState.Active) return;

        if (owner.hitReactionComponent.combatData.combatType == ECombatType.Dodge) return;

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Roll))
        {
            Vector3 direction = owner.characterMovement.GetSmoothDesiredMoveDirection;
            owner.dodgeComponent.DoRoll(direction);
        }
    }

    void Dash()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction ||
            owner.locomotionData.isGrounded ||
            owner.attackComponent.skillState == ESkillState.Playing) return;

        if (owner.wallRideComponent?.wallRideState == EWallRideState.Active) return;

        if (owner.hitReactionComponent.combatData.combatType == ECombatType.Dodge) return;

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Air_Dash))
        {
            Vector3 direction = owner.characterMovement.GetSmoothDesiredMoveDirection;
            owner.dodgeComponent.DoDash(direction);
        }
    }
}
