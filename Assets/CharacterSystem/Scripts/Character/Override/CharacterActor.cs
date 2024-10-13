using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActor : Character
{
    [Header("Audio")]
    [HideInInspector] public AudioSource characterAudio;

    [Header("[Components]")]
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public AnimationEvent animationEvent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AttackComponent attackComponent;
    [HideInInspector] public HitReactionComponent hitReactionComponent;
    [HideInInspector] public FreeFlowComponent freeFlowComponent;
    [HideInInspector] public CharacterMovement characterMovement;
    [HideInInspector] public SkillComponent skillComponent;
    [HideInInspector] public HitTraceComponent hitTraceComponent;
    [HideInInspector] public DodgeComponent dodgeComponent;
    [HideInInspector] public WallRideComponent wallRideComponent;

    [Header("[Locomotion]")]
    public LocomotionFSM locomotionFSM;
    public LocomotionData locomotionData;

    [Header("[Weapon]")]
    public WeaponBase mainWeapon;
    public WeaponBase secondWeapon;

    [Header("[Ragdoll]")]
    [SerializeField] List<Rigidbody> ragdollRigs;

    [Header("[Coroutine]")]
    public Coroutine C_Jump;
    public Coroutine C_LookOpposite;

    public int jumpCount;

    protected override void Awake()
    {
        base.Awake();

        Init();
    }

    protected override void Update()
    {
        if (!statsComponent.isAlive) return;

        base.Update();

        CheckGround();
        ApplyGravity();
        AirControl();

        UpdateAnimationState();
    }

    protected virtual void Init()
    {
        // Audio
        characterAudio = GetComponent<AudioSource>();

        // Components
        characterController = GetComponent<CharacterController>();
        animationEvent = GetComponent<AnimationEvent>();
        animator = GetComponent<Animator>();
        hitReactionComponent = GetComponent<HitReactionComponent>();
        freeFlowComponent = GetComponent<FreeFlowComponent>();
        characterMovement = GetComponent<CharacterMovement>();
        skillComponent = GetComponent<SkillComponent>();
        hitTraceComponent = GetComponent<HitTraceComponent>();
        dodgeComponent = GetComponent<DodgeComponent>();
        attackComponent = GetComponent<AttackComponent>();
        wallRideComponent = GetComponent<WallRideComponent>();

        // Locomotion
        locomotionFSM = new LocomotionFSM(this);
        locomotionFSM.Init(locomotionFSM.idleState);
        locomotionData = locomotionData.Clone();

        statsComponent.OnDeadAction += Dead;
    }

    public override void StopEveryCoroutines()
    {
        attackComponent?.StopAllCoroutines();
        hitReactionComponent?.StopAllCoroutines();
        freeFlowComponent?.StopAllCoroutines();
        characterMovement?.StopAllCoroutines();
        skillComponent?.StopAllCoroutines();
        hitTraceComponent?.StopAllCoroutines();
        dodgeComponent?.StopAllCoroutines();
        wallRideComponent?.StopAllCoroutines();
    }

    protected override void ApplyGravity()
    {
        if (locomotionData == null || !locomotionData.useGravity || !statsComponent.isAlive) return;

        switch (locomotionData.movementTypeState)
        {
            case EMovementTypeState.Grounded:
                locomotionData.jumpForce = 0f;
                characterController.Move(new Vector3(0f, locomotionData.gravity, 0f) * Time.deltaTime);
                break;
            case EMovementTypeState.InAir:
                locomotionData.jumpForce += locomotionData.gravity * Time.deltaTime;
                locomotionData.jumpForce = Mathf.Clamp(locomotionData.jumpForce, -100f, 100f);
                characterController.Move(new Vector3(0f, locomotionData.jumpCurve.Evaluate(locomotionData.jumpForce), 0f) * Time.deltaTime);
                break;
        }
    }

    protected override void CheckGround()
    {
        if (locomotionData == null) return;

        switch (locomotionData.movementTypeState)
        {
            case EMovementTypeState.Grounded:
                locomotionData.fallingTime = 0f;
                break;
            case EMovementTypeState.InAir:
                locomotionData.fallingTime += Time.deltaTime;
                break;
        }

        if (locomotionData.isJump) return;

        jumpCount = 0;
        locomotionData.isGrounded = Physics.CheckSphere(transform.position, characterController.radius, locomotionData.groundLayer.value);
        animator.SetBool(AnimationHash.HASH_GROUNDED, locomotionData.isGrounded);
        locomotionData.movementTypeState = locomotionData.isGrounded ? EMovementTypeState.Grounded : EMovementTypeState.InAir;
    }

    protected override void AirControl()
    {

    }

    public override void Dead(DamageInfo damageInfo)
    {
        Vector3 direction = Util.GetDirection(damageInfo.causer.transform.position, transform.position, false);
        if (useRagdoll)
        {
            animator.enabled = false;
            SetRagdoll(false);
            Rigidbody hipsRig = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
            if (hipsRig != null)
            {
                hipsRig.AddForce(direction * (hipsRig.mass * damageInfo.damageAmount), ForceMode.Impulse);
            }
        }
        else
        {
            animator.CrossFadeInFixedTime(AnimationHash.HASH_DEAD, 0.1f);
        }

        hitReactionComponent.combatData.combatType = ECombatType.Dead;
        characterController.enabled = false;
        statsComponent.isAlive = false;
        gameObject.layer = 0;
        characterTag = "None";

        StopAllCoroutines();
    }

    public void SetLocomotionState(ELocomotionState state)
    {
        switch (state)
        {
            case ELocomotionState.Stop:
                locomotionFSM.TransitionTo(locomotionFSM.idleState);
                break;

            case ELocomotionState.Walk:
                locomotionFSM.TransitionTo(locomotionFSM.walkState);
                break;

            case ELocomotionState.Sprint:
                locomotionFSM.TransitionTo(locomotionFSM.runState);
                break;
        }
    }

    IEnumerator Jump()
    {
        locomotionData.isJump = true;
        locomotionData.isGrounded = false;
        animator.SetBool(AnimationHash.HASH_GROUNDED, false);
        animator.SetBool(AnimationHash.HASH_IS_JUMP, true);
        locomotionData.movementTypeState = EMovementTypeState.InAir;
        locomotionData.jumpForce = locomotionData.maxJumpForce;
        if (jumpCount == 0)
        {
            animator.CrossFadeInFixedTime(AnimationHash.HASH_JUMP, 0.1f);
        }
        jumpCount++;

        float jumpTime = 0f;
        while (!locomotionData.isGrounded)
        {
            jumpTime += Time.deltaTime;
            if (jumpTime > 0.2f)
            {
                locomotionData.isJump = false;
                animator.SetBool(AnimationHash.HASH_IS_JUMP, false);
            }

            yield return null;
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (attackComponent != null)
        {
            attackComponent.ResetSkill();
        }
        hitReactionComponent.combatData.combatType = ECombatType.None;
        locomotionData.useGravity = true;
    }

    IEnumerator LookOpposite(Vector3 position, float duration)
    {
        Vector3 direction = Util.GetDirection(position, transform.position);
        float currentLerpTime = 0f;
        float lerTime = duration;

        while (currentLerpTime < lerTime && !locomotionData.isSprint)
        {
            currentLerpTime += Time.deltaTime;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-direction), (currentLerpTime / currentLerpTime));
            }

            yield return null;
        }
    }

    public void DoJump()
    {
        if (C_Jump != null) StopCoroutine(C_Jump);
        C_Jump = StartCoroutine(Jump());
    }

    public void DoLookOpposite(Vector3 position, float duration)
    {
        if (C_LookOpposite != null) StopCoroutine(C_LookOpposite);
        C_LookOpposite = StartCoroutine(LookOpposite(position, duration));
    }

    void InitRagdoll()
    {
        Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rig in rigs)
        {
            ragdollRigs.Add(rig);
            rig.isKinematic = true;
        }
    }

    public void SetRagdoll(bool isRagdoll)
    {
        ragdollRigs.ForEach(rig => rig.isKinematic |= isRagdoll);
    }

    public virtual void UpdateAnimationState()
    {
        if (locomotionFSM != null)
        {
            locomotionFSM.Update();
        }
    }
}
