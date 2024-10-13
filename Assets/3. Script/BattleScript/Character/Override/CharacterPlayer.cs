using UnityEngine;

public class CharacterPlayer : CharacterActor
{
    private void OnAnimatorMove()
    {
        if (attackComponent.skillState == ESkillState.Playing && freeFlowComponent.GetTarget() == null)
        {
            transform.position = animator.targetPosition;
        }
    }

    protected override void AirControl()
    {
        base.AirControl();

        Vector3 moveDirection = Vector3.ClampMagnitude(characterMovement.GetSmoothDesiredMoveDirection * characterMovement.GetStateSpeed() * locomotionData.airControl, locomotionData.currentMovementSettings.sprintSpeed);
        switch (locomotionData.movementTypeState)
        {
            case EMovementTypeState.InAir:
                characterController.Move(new Vector3(moveDirection.x, 0.0f, moveDirection.z) * Time.deltaTime);
                break;
        }
    }

    protected override void Init()
    {
        base.Init();

        characterMovement = GetComponent<CharacterMovement>();

        SetCharacterTag("Player");

        statsComponent.OnDeadAction += Dead;
        hitTraceComponent.OnHit += (Character hitCharacter, Vector3 hitPoint, Vector3 hitDirection) =>
        {
            if (hitTraceComponent.GetHitInfo().hitCharacters.Count > 0)
            {
                hitTraceComponent.GetHitInfo().hitCharacters.Sort((a, b) => Util.GetDistance(transform.position, a.transform.position).CompareTo(Util.GetDistance(transform.position, b.transform.position)));
                //if (hitTraceComponent.GetHitInfo().hitCharacters[0].statsComponent.isAlive)
                //{
                //    targetingComponent.OnTargetChanged?.Invoke(hitTraceComponent.GetHitInfo().hitCharacters[0]);
                //}
            }
            else
            {
                //targetingComponent.OnTargetChanged?.Invoke(hitCharacter);
            }
        };

        //characterAnim.SetFloat(AnimationParams.HASH_MOVE_TYPE, 0);
    }

    public override void Dead(DamageInfo damageInfo)
    {
        base.Dead(damageInfo);

        //PlayerController.Instance.enabled = false;
        characterMovement.enabled = false;
        //targetingComponent.OnTargetEnd?.Invoke();
    }
}
