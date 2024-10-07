using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterEnemy : Character
{
    [Header("[Character AI]")]
    public NavMeshAgent navMeshAgent;
    public AIPerception aiPerception;
    public BehaviorSets behaviorSets;

    [Header("[AI Data]")]
    private Vector3 smoothDesiredMoveDirection;
    private Vector3 randomPosition;

    [Header("[Coroutine]")]
    private Coroutine C_RandomMove;

    protected override void Update()
    {
        base.Update();

        if (statsComponent.isAlive)
        {
            if (navMeshAgent != null)
            {
                Vector3 direction = transform.position;

                direction = Util.GetDirection(transform.position, navMeshAgent.steeringTarget);

                if (navMeshAgent.enabled && navMeshAgent.desiredVelocity.magnitude > 1.0f && hitReactionComponent.combatData.combatType == ECombatType.None)
                {
                    switch (locomotionData.movementTypeState)
                    {
                        case EMovementTypeState.Grounded:
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * locomotionData.rotationSpeed);
                            break;

                        case EMovementTypeState.InAir:
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * locomotionData.rotationSpeed);
                            break;
                    }
                }
            }
        }
    }

    private void OnAnimatorMove()
    {
        if (attackComponent != null && attackComponent.skillState == ESkillState.Playing)
        {
            transform.position = animator.targetPosition;
        }
    }

    protected override void CheckGround()
    {
        base.CheckGround();

        switch (locomotionData.movementTypeState)
        {
            case EMovementTypeState.Grounded:
                if (navMeshAgent != null)
                {
                    navMeshAgent.enabled = true;
                }
                break;

            case EMovementTypeState.InAir:
                if (navMeshAgent != null)
                {
                    navMeshAgent.enabled = false;
                }
                break;
        }
    }

    protected override void AirControl()
    {
        base.AirControl();

        Vector3 moveDirection = Vector3.ClampMagnitude(smoothDesiredMoveDirection * locomotionData.airControl, locomotionData.currentMovementSettings.sprintSpeed);
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

        navMeshAgent = GetComponent<NavMeshAgent>();
        aiPerception = GetComponent<AIPerception>();
        behaviorSets = GetComponent<BehaviorSets>();

        statsComponent.OnDeadAction += Dead;
        //characterAnim.SetFloat(AnimationParams.HASH_MOVE_TYPE, 1);
        navMeshAgent.acceleration = locomotionData.maxAcceleration;

        //===================

        SetCharacterTag("Enemy");
        statsComponent.OnDeadAction += Dead;

        //if (mainWeapon != null)
        //{
        //    mainWeapon.Equip();
        //}
        //
        //if (secondWeapon != null)
        //{
        //    secondWeapon.Equip();
        //}
    }

    public override void UpdateAnimationState()
    {
        base.UpdateAnimationState();

        locomotionData.angle = Vector3.SignedAngle(transform.forward, navMeshAgent.desiredVelocity, Vector3.up);
        animator.SetFloat(AnimationHash.HASH_LOCOMOTION_SPEED, navMeshAgent.desiredVelocity.magnitude);

        if (animator.GetFloat(AnimationHash.HASH_LOCOMOTION_SPEED) > 0.0f || (navMeshAgent.enabled && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)) ||
                hitReactionComponent.combatData.combatType != ECombatType.None)
        {
            locomotionData.currentMovementSettings.currentSpeed = 0.0f;
        }
        else
        {
            switch (locomotionData.locomotionState)
            {
                case ELocomotionState.Stop:
                    locomotionData.currentMovementSettings.currentSpeed =
                        Mathf.Lerp(locomotionData.currentMovementSettings.currentSpeed, 0.0f, Time.smoothDeltaTime * locomotionData.GetMaxDamp());
                    break;

                case ELocomotionState.Walk:
                    locomotionData.currentMovementSettings.currentSpeed =
                        Mathf.Lerp(locomotionData.currentMovementSettings.currentSpeed,
                        locomotionData.currentMovementSettings.walkSpeed, Time.smoothDeltaTime);
                    break;

                case ELocomotionState.Sprint:
                    locomotionData.currentMovementSettings.currentSpeed =
                        Mathf.Lerp(locomotionData.currentMovementSettings.currentSpeed,
                        locomotionData.currentMovementSettings.sprintSpeed, Time.smoothDeltaTime);
                    break;
            }
        }

        navMeshAgent.speed = locomotionData.currentMovementSettings.currentSpeed;

        switch (locomotionData.movementTypeState)
        {
            case EMovementTypeState.Grounded:
                smoothDesiredMoveDirection = Vector3.Lerp(smoothDesiredMoveDirection, navMeshAgent.desiredVelocity, Time.deltaTime * locomotionData.GetMaxAcceleration());
                if (navMeshAgent.autoBraking)
                {
                    characterController.Move(navMeshAgent.desiredVelocity * locomotionData.ratio * Time.deltaTime);
                }
                else
                {
                    characterController.Move(smoothDesiredMoveDirection * locomotionData.ratio * Time.deltaTime);
                }
                break;
        }
    }

    public override void Dead(DamageInfo damageInfo)
    {
        base.Dead(damageInfo);

        behaviorSets.enabled = false;
        aiPerception.enabled = false;
        navMeshAgent.enabled = false;
    }

    public void PlayCombo(SO_Skill combo)
    {
        attackComponent.SetSaveCombo(combo);
    }

    private IEnumerator RandomMove(Vector3 centerPosition)
    {
        while (true)
        {
            float waitForInplace = 1.0f;
            yield return new WaitForSeconds(waitForInplace);
            SetDestination(GetRandomPoint(centerPosition));
            yield return new WaitWhile(() => !navMeshAgent.enabled || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance);
        }
    }

    public void SetDestination(Vector3 movePosition)
    {
        if (!navMeshAgent.enabled) return;

        navMeshAgent.destination = movePosition;
    }

    public void SetRandomMove(Vector3 centerPosition)
    {
        StopRandomMove();
        C_RandomMove = StartCoroutine(RandomMove(centerPosition));
    }

    public void StopRandomMove()
    {
        if (C_RandomMove != null) StopCoroutine(C_RandomMove);
    }

    public Vector3 GetRandomPoint()
    {
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * Random.Range(5.0f, 10.0f);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, 10.0f, NavMesh.AllAreas))
        {
            this.randomPosition = hit.position;
        }

        return this.randomPosition;
    }

    public Vector3 GetRandomPoint(Vector3 centerPosition)
    {
        Vector3 randomPosition = centerPosition + Random.insideUnitSphere * Random.Range(5.0f, 10.0f);
        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 10.0f, NavMesh.AllAreas))
        {
            this.randomPosition = hit.position;
        }

        return this.randomPosition;
    }
}
