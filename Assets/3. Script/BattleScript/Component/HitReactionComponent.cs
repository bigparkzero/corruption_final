using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECombatStanceType
{
    DefaultStance,
    AttackStance,
}

public enum ECombatType
{
    None,
    Attack,
    Dodge,
    HitReaction,
    Dead,
}

public enum EAttackDirection
{
    Forward,
    Back,
    Left,
    Right,
    Up,
    Down,
}

[System.Serializable]
public struct CombatData
{
    [Header("[Combat Data]")]
    public ECombatStanceType combatStanceType;
    public ECombatType combatType;
    public EAttackDirection attackDirection;

    public bool isAiming;
    public bool isAttackable { get => combatType == ECombatType.None; }
}

[System.Serializable]
public struct HitAnimation
{
    [Header("[Hit Animation]")]
    public AnimationClip hitClip;
    public float transitionDuration;
}

[System.Serializable]
public struct HitFeedback
{
    [Header("[Hit Feedback]")]
    public bool isHitStop;
    public float stopScale;
    public float stopTime;
    public bool isShake;
    public float shakeScale;
}

[System.Serializable]
public struct HitKnockback
{
    [Header("[Hit Knockback]")]
    public bool isKnockback;
    public float knockbackForce;
    public float knockbackTime;
    public float intensity;
    public bool wallHitable;
}

[System.Serializable]
public struct HitAirborne
{
    [Header("[Hit Airborne]")]
    public bool isAirborne;
    public float height;
    public float force;
    public float airborneTime;
    public float intensity;
}

[System.Serializable]
public struct HitParticleFX
{
    [Header("[Hit ParticleFX]")]
    public ParticleSystem hitParticle;
    public ParticleSystem blockHitParticle;
    public ParticleSystem parryParticle;
    public Vector3 scale;
    public Vector3 eulerOffset;
    public bool isParent;
}

[System.Serializable]
public struct AttackParticleFX
{
    [Header("[Attack ParticleFX]")]
    public GameObject attackParticle;
    public bool useParent;
    public Vector3 offset;
    public Vector3 eulerOffset;
    public Vector3 scale;
    public float destroyTime;
}

[System.Serializable]
public struct HitSoundFX
{
    [Header("[Hit SoundFX]")]
    public List<AudioClip> hitClips;
    public float volume;

    public void Play(AudioSource source, List<AudioClip> clips)
    {
        if (source == null || clips.Count <= 0)
        {
            Debug.Log("Hit Clip is null");
            return;
        }

        source.PlayOneShot(clips[Random.Range(0, clips.Count)], volume);
    }
}

[System.Serializable]
public struct HitReactionData
{
    [Header("[Hit Trace Data]")]
    public TraceInfo traceInfo;

    [Header("[Hit Reaction Data]")]
    public EAttackDirection attackDirection;
    public HitAnimation hitAnimation;
    public HitFeedback HitFeedback;
    public HitKnockback HitKnockback;
    public HitAirborne HitAirborne;
    public HitParticleFX HitParticleFX;
    public HitSoundFX HitSoundFX;
    public AttackParticleFX attackParticleFX;
    public float Damage;
}

[System.Serializable]
public struct HitReactionDatas
{
    [Header("[Hit Reaction Datas]")]
    public List<HitReactionData> hitReactionData;
}

public class HitReactionComponent : MonoBehaviour
{
    [Header("[Component]")]
    Character owner;

    [Header("[Hit Reaction Component]")]
    public CombatData combatData;
    [SerializeField] HitReactionData hitReactionData;
    [SerializeField] int maxIndex;
    [SerializeField] int currentIndex = 0;

    [Header("[Timer]")]
    float T_AirborneTimer;

    [Header("[Coroutine]")]
    Coroutine C_Knockback;
    Coroutine C_Airborne;
    Coroutine C_AirborneTimer;
    Coroutine C_HitStop;

    private void Start()
    {
        owner = GetComponent<Character>();
    }

    public void SetHitReaction(List<HitReactionData> data)
    {
        hitReactionData = data[currentIndex];
        combatData.attackDirection = hitReactionData.attackDirection;
        owner.hitTraceComponent.SetTraceInfo(hitReactionData.traceInfo);

        currentIndex++;
        if (currentIndex >= maxIndex)
        {
            ResetIndex();
        }
    }

    public void SetMaxIndex(AnimationClip clip)
    {
        maxIndex = Util.GetAnimEventCount(clip, "Anim_OnAttack");
        ResetIndex();
    }

    public void ResetIndex()
    {
        currentIndex = 0;
    }

    public HitReactionData GetHitReaction()
    {
        return hitReactionData;
    }

    #region Combat

    public IEnumerator Knockback(HitKnockback hitKnockback, Character causer)
    {
        float knockbackTime = hitKnockback.knockbackTime;
        Vector3 direction = Util.GetDirection(causer.transform.position, owner.transform.position);
        Vector3 startPosition = owner.transform.position;
        Vector3 endPosition = startPosition + (direction * hitKnockback.knockbackForce);

        while (knockbackTime > 0f)
        {
            knockbackTime -= Time.deltaTime;

            float distanceFromEnd = Util.GetDistance(owner.transform.position, endPosition);
            float totalDistance = Util.GetDistance(startPosition, endPosition);
            float ratio = distanceFromEnd / totalDistance;

            //if (Physics.SphereCast(owner.transform.position + owner.transform.TransformDirection(0.0f, 1.0f, 0.0f),
            //        (owner.characterController.radius * owner.transform.lossyScale.x) * 0.25f, direction, out RaycastHit hitInfo,
            //        (owner.characterController.radius * owner.transform.lossyScale.x), owner.locomotionData.groundLayer.value))
            //{
            //    if (hitInfo.collider && hitKnockback.wallHitable)
            //    {
            //        Vector3 normal = hitInfo.normal;
            //        normal.y = 0.0f;
            //        owner.transform.rotation = Quaternion.LookRotation(normal);
            //        yield break;
            //    }
            //}

            owner.characterController.Move(((direction * hitKnockback.knockbackForce) * (Time.deltaTime * hitKnockback.intensity)) * ratio);
            yield return null;
        }
    }

    private IEnumerator Airborne(HitAirborne hitAirborne, Character hitActor = null)
    {
        owner.locomotionData.isJump = true;
        owner.locomotionData.isGrounded = false;
        owner.animator.SetBool(AnimationHash.HASH_GROUNDED, false);
        owner.locomotionData.movementTypeState = EMovementTypeState.InAir;

        if (hitActor != null)
        {
            SetAirborneTimer(hitActor, hitAirborne.airborneTime);
        }

        float elapsedTime = 0.0f;
        Vector3 startPosition = owner.transform.position;
        Vector3 forward = (owner == hitActor ? -owner.transform.forward : owner.transform.forward) * hitAirborne.force;
        Vector3 movePosition = new Vector3(forward.x, hitAirborne.height, forward.z);
        Debug.DrawLine(startPosition, owner.transform.position + movePosition, Color.blue, 3.0f);
        while (!owner.locomotionData.isGrounded)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 0.1f)
                owner.locomotionData.isJump = false;

            if (owner.transform.position.y >= startPosition.y + hitAirborne.height && hitAirborne.height > 0.0f)
            {
                owner.locomotionData.isJump = false;
                yield break;
            }

            owner.characterController.Move(movePosition * (Time.deltaTime * hitAirborne.intensity));
            yield return null;
        }
    }

    private IEnumerator AirborneTimer(Character target, float timer)
    {
        target.hitReactionComponent.T_AirborneTimer = Time.time + timer;
        target.animationEvent.Anim_GravityDisable();
        yield return new WaitWhile(() => target.hitReactionComponent.T_AirborneTimer > Time.time && !target.locomotionData.isGrounded);
        target.animationEvent.Anim_GravityEnable();
    }

    private IEnumerator HitStop(float stopTime, float stopScale)
    {
        Time.timeScale = stopScale;
        yield return new WaitForSecondsRealtime(stopTime);
        Time.timeScale = 1.0f;
    }

    public void SetKnockback(HitKnockback hitKnockback, Character causer)
    {
        if (!hitKnockback.isKnockback) return;

        if (C_Knockback != null) StopCoroutine(C_Knockback);
        C_Knockback = StartCoroutine(Knockback(hitKnockback, causer));
    }

    public void SetAirborne(HitAirborne hitAirborne, Character hitActor = null)
    {
        if (!hitAirborne.isAirborne) return;

        if (hitActor != null && hitActor.locomotionData.isGrounded)
        {
            if (hitActor.hitReactionComponent.C_Knockback != null)
                hitActor.hitReactionComponent.StopCoroutine(hitActor.hitReactionComponent.C_Knockback);
        }
        if (C_Airborne != null) StopCoroutine(C_Airborne);
        C_Airborne = StartCoroutine(Airborne(hitAirborne, hitActor));
    }

    public void SetAirborneTimer(Character target, float timer)
    {
        if (target.hitReactionComponent.C_AirborneTimer != null) target.StopCoroutine(target.hitReactionComponent.C_AirborneTimer);
        target.hitReactionComponent.C_AirborneTimer = target.StartCoroutine(AirborneTimer(target, timer));
    }

    public void SetHitStop(float stopTime, float stopScale)
    {
        if (C_HitStop != null) StopCoroutine(C_HitStop);
        C_HitStop = StartCoroutine(HitStop(stopTime, stopScale));
    }

    //public void PlayCameraShake()
    //{
    //    if (!hitReactionData.HitFeedback.isShake) return;
    //
    //    CameraManager.Instance.CameraShake(GetShakeDirection());
    //}
    //
    //public Vector3 GetShakeDirection()
    //{
    //    Vector3 direction = new Vector3(0.0f, 0.0f, 1.0f);
    //
    //    switch (combatData.attackDireciton)
    //    {
    //        case EAttackDirection.Front:
    //            direction = new Vector3(0.0f, 0.0f, 1.0f);
    //            break;
    //
    //        case EAttackDirection.Back:
    //            direction = new Vector3(0.0f, 0.0f, -1.0f);
    //            break;
    //
    //        case EAttackDirection.Right:
    //            direction = new Vector3(1.0f, 0.0f, 0.0f);
    //            break;
    //
    //        case EAttackDirection.Left:
    //            direction = new Vector3(-1.0f, 0.0f, 0.0f);
    //            break;
    //
    //        case EAttackDirection.Up:
    //            direction = new Vector3(0.0f, 1.0f, 0.0f);
    //            break;
    //
    //        case EAttackDirection.Down:
    //            direction = new Vector3(0.0f, -1.0f, 0.0f);
    //            break;
    //    }
    //
    //    return direction * hitReactionData.HitFeedback.shakeScale;
    //}

    #endregion

    #region Animation

    public void HitAnimation(Character hitActor)
    {
        if (hitReactionData.hitAnimation.hitClip == null) return;

        hitActor.animator.CrossFadeInFixedTime(hitReactionData.hitAnimation.hitClip.name, hitReactionData.hitAnimation.transitionDuration);
    }

    #endregion

    #region Effect & Particle

    public void SpawnAttackFX()
    {
        if (hitReactionData.attackParticleFX.attackParticle == null) return;

        if (hitReactionData.attackParticleFX.attackParticle.GetComponent<ParticleSystem>() != null)
        {
            ParticleSystem particle = hitReactionData.attackParticleFX.attackParticle.GetComponent<ParticleSystem>();
            Vector3 spawnPosition = owner.transform.position + owner.transform.TransformDirection(hitReactionData.attackParticleFX.offset);
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(hitReactionData.attackParticleFX.eulerOffset);
            ParticleSystem effect = Instantiate(particle, spawnPosition, spawnRotation);
            if (hitReactionData.attackParticleFX.useParent)
            {
                effect.transform.SetParent(owner.transform, true);
            }
            effect.transform.localScale = hitReactionData.attackParticleFX.scale;
            Destroy(effect.gameObject, hitReactionData.attackParticleFX.destroyTime <= 0.0f ? effect.main.startLifetime.constantMax : hitReactionData.attackParticleFX.destroyTime);
        }
        else
        {
            Vector3 spawnPosition = owner.transform.position + owner.transform.TransformDirection(hitReactionData.attackParticleFX.offset);
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(hitReactionData.attackParticleFX.eulerOffset);
            GameObject effect = Instantiate(hitReactionData.attackParticleFX.attackParticle, spawnPosition, spawnRotation);
            if (hitReactionData.attackParticleFX.useParent)
            {
                effect.transform.SetParent(owner.transform, true);
            }
            effect.transform.localScale = hitReactionData.attackParticleFX.scale;
            Destroy(effect, hitReactionData.attackParticleFX.destroyTime);
        }
    }

    #endregion
}
