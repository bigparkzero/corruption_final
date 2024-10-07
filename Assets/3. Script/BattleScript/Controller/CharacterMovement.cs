using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("[Character Movement]")]
    public Character owner;
    private Vector3 inputVelocity;
    private Vector3 inputDirection;
    private Vector3 desiredMoveDirection;
    private Vector3 lastDesiredMoveDirection;
    private Vector3 strafeMoveDirection;
    private Vector3 smoothDesiredMoveDirection;
    private Vector3 smoothLastDesiredMoveDirection;

    public Vector3 GetInputDirection { get => inputDirection; }
    public Vector3 GetDesiredMoveDirection { get => desiredMoveDirection; }
    public Vector3 GetSmoothDesiredMoveDirection { get => smoothDesiredMoveDirection; }
    public Vector3 GetStrafeMoveDirection { get => strafeMoveDirection; }

    private void Start()
    {
        owner = GetComponent<Character>();
    }

    private void Update()
    {
        SetEssentialValues();
        UpdateCharacterInfo();
        SetMovementState();
    }

    private void SetEssentialValues()
    {
        owner.locomotionData.acceleration = owner.locomotionData.CalculateAcceleraction();
        owner.locomotionData.speed = new Vector3(owner.locomotionData.velocity.x, 0.0f, owner.locomotionData.velocity.z).magnitude;
        owner.locomotionData.isMoving = owner.locomotionData.speed > 0.1f;
        //owner.characterAnim.SetBool(AnimationParams.HASH_MOVING, owner.locomotionData.isMoving);

        owner.locomotionData.inputAmount = inputVelocity.magnitude;
        owner.locomotionData.hasInput = owner.locomotionData.inputAmount > 0.0f;
        //owner.characterAnim.SetBool(AnimationParams.HASH_HAS_INPUT, owner.locomotionData.hasInput);
        owner.locomotionData.movementInputAmount = owner.locomotionData.GetCurrentAcceleration().magnitude / owner.locomotionData.GetMaxAcceleration();
        owner.locomotionData.hasMovementInput = owner.locomotionData.movementInputAmount > 0.0f;

        if (owner.locomotionData.hasInput)
        {
            lastDesiredMoveDirection = desiredMoveDirection;
            smoothLastDesiredMoveDirection = smoothDesiredMoveDirection;
        }
    }

    private void UpdateCharacterInfo()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

        // 키 입력 값
        inputVelocity = move;

        var forward = Camera.main.transform.forward;
        var right = Camera.main.transform.right;
        forward.y = 0.0f;
        right.y = 0.0f;
        forward.Normalize();
        right.Normalize();

        // 키 입력 방향 값
        inputDirection = (forward * inputVelocity.z) + (right * inputVelocity.x);
        inputDirection.Normalize();

        // 캐릭터 기준 방향 값
        desiredMoveDirection = inputDirection;
        desiredMoveDirection.Normalize();

        smoothDesiredMoveDirection = Vector3.Lerp(smoothDesiredMoveDirection, desiredMoveDirection, Time.deltaTime * owner.locomotionData.GetMaxAcceleration());

        strafeMoveDirection = new Vector3(desiredMoveDirection.x * transform.right.x + desiredMoveDirection.z * transform.right.z, 0.0f,
            desiredMoveDirection.x * transform.forward.x + desiredMoveDirection.z * transform.forward.z) * GetStateSpeed();

        owner.animator.SetFloat(AnimationHash.HASH_LOCOMOTION_SPEED, GetStateSpeed());
        if (owner.attackComponent.skillState == ESkillState.Playing) return;

        if (owner.locomotionData.isGrounded)
        {
            // 캐릭터 이동
            owner.characterController.Move(smoothDesiredMoveDirection * (GetStateSpeed() * owner.locomotionData.ratio) * Time.deltaTime);

            if (!owner.locomotionData.hasInput)
            {
                owner.SetLocomotionState(ELocomotionState.Stop);
            }
            else
            {
                if (!owner.locomotionData.isSprint)
                {
                    //if (owner.characterOptional.walkToggle)
                    //{
                    //    owner.SetLocomotionState(ELocomotionState.Walk);
                    //}
                    //else
                    //{
                        owner.SetLocomotionState(ELocomotionState.Walk);
                    //}
                }
                else
                {
                    owner.SetLocomotionState(ELocomotionState.Sprint);
                }
            }
        }

        owner.locomotionData.velocity = owner.locomotionData.GetVelocity(owner);
    }

    private void SetMovementState()
    {
        switch (owner.locomotionData.movementTypeState)
        {
            case EMovementTypeState.Grounded:
                UpdateGroundedRotation();
                break;

            case EMovementTypeState.InAir:
                UpdateInAirRotation();
                UpdateInAirValues();
                DistanceMatchLanding();
                break;
        }
    }

    private void UpdateGroundedRotation()
    {
        //if (owner.locomotionData.useOrientedRotation || !owner.locomotionData.hasInput /* || owner.characterAnim.GetFloat(AnimationParams.HASH_MOVE_SPEED) > 0.0f */) return;

        //switch (owner.locomotionData.moveMode)
        //{
        //    case EMoveMode.Directional:
        //        if (owner.locomotionData.isMoving)
        //        {
        //            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(smoothLastDesiredMoveDirection), Time.deltaTime * owner.locomotionData.rotationSpeed);
        //        }
        //        break;

        //        //    case EMoveMode.Strafe:
        //        if (owner.locomotionData.isMoving)
        //        {
        //            if (owner.targetingComponent.GetTarget() == null)
        //            {
        //                Vector3 cameraForward = Camera.main.transform.forward;
        //                cameraForward.y = 0.0f;
        //                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(cameraForward), Time.deltaTime * owner.locomotionData.rotationSpeed);
        //            }
        //            owner.locomotionData.direction = Vector3.SignedAngle(owner.transform.forward, smoothLastDesiredMoveDirection, Vector3.up);
        //        }
        //        else
        //        {
        //            owner.locomotionData.direction = Vector3.SignedAngle(owner.transform.forward, Camera.main.transform.forward, Vector3.up);
        //        }
        //        break;
        //}
        if (owner.locomotionData.isMoving)
        {
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(smoothLastDesiredMoveDirection), Time.deltaTime * owner.locomotionData.rotationSpeed);
        }
    }

    private void UpdateInAirRotation()
    {
        //if (owner.locomotionData.useOrientedRotation || !owner.locomotionData.hasInput /* || owner.characterAnim.GetFloat(AnimationParams.HASH_MOVE_SPEED) > 0.0f */) return;

        //switch (owner.locomotionData.moveMode)
        //{
        //    case EMoveMode.Directional:
        //        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(smoothLastDesiredMoveDirection), Time.deltaTime * owner.locomotionData.airRotationSpeed);
        //        break;
        //
        //    case EMoveMode.Strafe:
        //
        //        break;
        //}
        if (smoothLastDesiredMoveDirection == Vector3.zero) return;
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(smoothLastDesiredMoveDirection), Time.deltaTime * owner.locomotionData.rotationSpeed);
    }

    private void UpdateInAirValues()
    {
        owner.locomotionData.fallSpeed = owner.locomotionData.jumpForce;
    }

    public float GetStateSpeed()
    {
        //if (owner.characterAnim.GetFloat(AnimationParams.HASH_MOVE_SPEED) > 0.0f)
        //{
        //    owner.locomotionData.currentMovementSettings.currentSpeed = 0.0f;
        //}
        //else
        //{
        //    switch (owner.locomotionData.locomotionState)
        //    {
        //        case ELocomotionState.Idle:
        //            owner.locomotionData.currentMovementSettings.currentSpeed =
        //                Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed, 0.0f, Time.smoothDeltaTime * owner.locomotionData.GetMaxBrakingDeceleration());
        //            break;
        //
        //        case ELocomotionState.Walk:
        //            owner.locomotionData.currentMovementSettings.currentSpeed =
        //                Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed,
        //                owner.locomotionData.animationCurveData.walkCurve.Evaluate(owner.locomotionData.currentMovementSettings.walkSpeed), Time.smoothDeltaTime);
        //            break;
        //
        //        //case ELocomotionState.Jog:
        //        //    owner.locomotionData.currentMovementSettings.currentSpeed =
        //        //        Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed,
        //        //        owner.locomotionData.animationCurveData.jogCurve.Evaluate(owner.locomotionData.currentMovementSettings.jogSpeed), Time.smoothDeltaTime);
        //        //    break;
        //
        //        case ELocomotionState.Sprint:
        //            owner.locomotionData.currentMovementSettings.currentSpeed =
        //                Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed,
        //                owner.locomotionData.animationCurveData.sprintCurve.Evaluate(owner.locomotionData.currentMovementSettings.sprintSpeed), Time.smoothDeltaTime);
        //            break;
        //    }
        //}
        switch (owner.locomotionData.locomotionState)
        {
            case ELocomotionState.Stop:
                owner.locomotionData.currentMovementSettings.currentSpeed =
                    Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed, 0.0f, Time.smoothDeltaTime * owner.locomotionData.GetMaxDamp());
                break;

            case ELocomotionState.Walk:
                owner.locomotionData.currentMovementSettings.currentSpeed = owner.locomotionData.currentMovementSettings.walkSpeed;
                //    Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed,
                //    owner.locomotionData.animationCurveData.walkCurve.Evaluate(owner.locomotionData.currentMovementSettings.walkSpeed), Time.smoothDeltaTime);
                break;

            //case ELocomotionState.Jog:
            //    owner.locomotionData.currentMovementSettings.currentSpeed =
            //        Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed,
            //        owner.locomotionData.animationCurveData.jogCurve.Evaluate(owner.locomotionData.currentMovementSettings.jogSpeed), Time.smoothDeltaTime);
            //    break;

            case ELocomotionState.Sprint:
                owner.locomotionData.currentMovementSettings.currentSpeed = owner.locomotionData.currentMovementSettings.sprintSpeed;
                //    Mathf.Lerp(owner.locomotionData.currentMovementSettings.currentSpeed,
                //    owner.locomotionData.animationCurveData.sprintCurve.Evaluate(owner.locomotionData.currentMovementSettings.sprintSpeed), Time.smoothDeltaTime);
                break;
        }
        return owner.locomotionData.currentMovementSettings.currentSpeed;
    }

    private void DistanceMatchLanding()
    {
        //float maxDistance = Mathf.Clamp(owner.locomotionData.maxJumpForce * owner.characterOptional.jumpCount, owner.locomotionData.maxJumpForce, owner.locomotionData.maxJumpForce * owner.characterOptional.jumpCount);
        //if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, maxDistance, owner.locomotionData.groundLayer.value))
        //{
        //    float currentDistance = Util.GetDistance(transform.position, hitInfo.point, false);
        //    float landing = owner.locomotionData.landingCurve.Evaluate(Mathf.Clamp01(1.0f - (currentDistance / maxDistance)));
        //    owner.animator.SetFloat(AnimationHash.HASH_LANDING, landing);
        //}
    }
}
