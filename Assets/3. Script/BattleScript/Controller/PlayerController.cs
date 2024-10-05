using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Character owner;

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
        owner = GetComponent<Character>();

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
            owner.hitReactionComponent.combatData.combatType == ECombatType.Dodge) return;

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

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Attack))
        {
            owner.attackComponent.SetInput(EKeyName.Attack);
        }
    }

    void SpecialSkill()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction) return;

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Skill))
        {
            owner.attackComponent.SetInput(EKeyName.Skill);
        }
    }

    void Roll()
    {
        if (owner.hitReactionComponent.combatData.combatType == ECombatType.HitReaction ||
            !owner.locomotionData.isGrounded ||
            owner.attackComponent.skillState == ESkillState.Playing) return;

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

        if (PlayerInputManager.Instance.IsKeyDown(EKeyName.Air_Dash))
        {
            Vector3 direction = owner.characterMovement.GetSmoothDesiredMoveDirection;
            owner.dodgeComponent.DoDash(direction);
        }
    }
}
