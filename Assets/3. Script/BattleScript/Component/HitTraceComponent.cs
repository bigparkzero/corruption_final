using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EArmedType
{
    None = 0,
    Main,
    Second,
}

public enum ETraceType
{
    Sphere = 0,
    Capsule,
    Box,
}

[Serializable]
public struct TraceInfo
{
    [Header("[Trace Info]")]
    public EArmedType armedType;
    public ETraceType traceType;
    public HumanBodyBones bones;
    public float traceRadius;
    public float traceAngle;
    public float traceHeight;

    public TraceInfo(EArmedType armedType, ETraceType traceType, HumanBodyBones bones, float traceRadius, float traceAngle, float traceHeight)
    {
        this.armedType = armedType;
        this.traceType = traceType;
        this.bones = bones;
        this.traceRadius = traceRadius;
        this.traceAngle = traceAngle;
        this.traceHeight = traceHeight;
    }
}

[Serializable]
public struct HitInfo
{
    [Header("[Hit Info]")]
    public Vector3 hitPoint;
    public Collider[] hitColliders;
    public Character hitCharacter;
    public List<Character> hitCharacters;
}

public class HitTraceComponent : MonoBehaviour
{
    CharacterActor owner;

    [Header("[Hit Trace Component]")]
    [SerializeField] LayerMask hitLayer;
    [SerializeField] string ignoreTag;

    [Header("[Hit Trace Info]")]
    [SerializeField] TraceInfo traceInfo;
    [SerializeField] HitInfo hitInfo;
    [SerializeField] int maxCount = 32;
    [SerializeField] bool isTrace = false;
    Transform startSlot, endSlot;
    public bool IsTrace { get => isTrace; }

    [Header("[Hit Trace Event]")]
    public UnityEvent OnReceiveDamage;
    public UnityAction<Character, Vector3, Vector3> OnHit;

    public GameObject prefab_hitFlare;
    public bool useHitFlare = true;

    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        PlayTrace();
    }

    void Init()
    {
        owner = GetComponent<CharacterActor>();

        hitInfo.hitColliders = new Collider[maxCount];
        hitInfo.hitCharacters = new List<Character>(maxCount);

        OnHit += HitEvent;
    }

    #region Trace

    bool CheckHitCharacter(Character character)
    {
        if (hitInfo.hitCharacters.Contains(character) || character == owner)
        {
            return true;
        }
        else
        {
            hitInfo.hitCharacters.Add(character);
            return false;
        }
    }

    public void SetTraceInfo(TraceInfo traceInfo)
    {
        this.traceInfo = traceInfo;
    }

    public TraceInfo GetTraceInfo()
    {
        return traceInfo;
    }

    void PlayTrace()
    {
        if (isTrace)
        {
            SkillData skill = owner.attackComponent.CurrentSkillData;
            if (skill.hitReactionDatas == null)
            {
                owner.attackComponent.ResetSkill();
                return; 
            }
            HitReactionDatas hitReactionDatas = skill.hitReactionDatas[owner.attackComponent.CurrentSkillIndex];
            HitReactionData hitReactionData = hitReactionDatas.hitReactionData[0];
            EArmedType armedType = hitReactionData.traceInfo.armedType;

            int count = 0;
            Vector3 detectPosition;
            switch (armedType)
            {
                case EArmedType.None:
                    detectPosition = owner.characterController.center;
                    break;
                case EArmedType.Main:
                    detectPosition = owner.mainWeapon.transform.position;
                    break;
                case EArmedType.Second:
                    detectPosition = owner.secondWeapon.transform.position;
                    break;
                default:
                    throw new Exception("invalid aremd type!");
            }
            count = Physics.OverlapSphereNonAlloc(detectPosition, traceInfo.traceRadius, hitInfo.hitColliders, hitLayer.value, QueryTriggerInteraction.Ignore);

            //switch (traceInfo.traceType)
            //{
            //    case ETraceType.Sphere:
            //        Vector3 detectPosition;
            //        switch (armedType)
            //        {
            //            case EArmedType.None:
            //                detectPosition = owner.characterController.center;
            //                break;
            //            case EArmedType.Main:
            //                detectPosition = owner.mainWeapon.transform.position;
            //                break;
            //            case EArmedType.Second:
            //                detectPosition = owner.secondWeapon.transform.position;
            //                break;
            //        }
            //        count = Physics.OverlapSphereNonAlloc(detectPosition, traceInfo.traceRadius, hitInfo.hitColliders, hitLayer.value, QueryTriggerInteraction.Ignore);
            //        break;
            //
            //    case ETraceType.Capsule:
            //        count = Physics.OverlapCapsuleNonAlloc(startSlot.position, endSlot.position, traceInfo.traceRadius, hitInfo.hitColliders, hitLayer.value, QueryTriggerInteraction.Ignore);
            //        break;
            //
            //    case ETraceType.Box:
            //        Vector3 centerPosition = Util.GetCenterPosition(startSlot.position, endSlot.position);
            //        Vector3 boxSize = new Vector3(traceInfo.traceRadius, traceInfo.traceRadius, traceInfo.traceRadius);
            //        count = Physics.OverlapBoxNonAlloc(centerPosition, boxSize * 0.5f, hitInfo.hitColliders, transform.rotation, hitLayer.value, QueryTriggerInteraction.Ignore);
            //        break;
            //}

            for (int i = 0; i < count; ++i)
            {
                Collider coll = hitInfo.hitColliders[i];
                print(coll.name);
                Vector3 closest = coll.ClosestPoint(detectPosition);
                Vector3 hitDirection = (closest - detectPosition).normalized;
                if (hitDirection == Vector3.zero) hitDirection = (coll.transform.position - transform.position).normalized;

                var hitActor = coll.gameObject.GetComponentInParent<Character>();
                if (hitActor != null && !CheckHitCharacter(hitActor) && !hitActor.HasTag(ignoreTag))
                {
                    PlayHitFlare(closest, hitDirection);
                    OnHit?.Invoke(hitActor, closest, hitDirection);
                }
            }
        }
    }

    public void PlayHitFlare(Vector3 hitPoint, Vector3 hitDirection)
    {
        if (!useHitFlare || prefab_hitFlare == null) return;

        Quaternion rotation = Quaternion.LookRotation(-hitDirection);
        GameObject flare = Instantiate(prefab_hitFlare, hitPoint, rotation);
        Destroy(flare, 1f);
    }

    public void OnTraceBegin()
    {
        switch (traceInfo.armedType)
        {
            case EArmedType.None:
                startSlot = owner.animator.GetBoneTransform(traceInfo.bones);
                endSlot = owner.animator.GetBoneTransform(traceInfo.bones);
                owner.mainWeapon.weaponInfo.weaponVFX.SetTrail(true);
                owner.mainWeapon.weaponInfo.weaponSFX.PlayAttackSFX(owner, 1.0f);
                owner.secondWeapon.weaponInfo.weaponVFX.SetTrail(true);
                owner.secondWeapon.weaponInfo.weaponSFX.PlayAttackSFX(owner, 1.0f);
                break;
        
            case EArmedType.Main:
                if (owner.mainWeapon == null)
                {
                    Debug.LogError("Main Weapon is null");
                    return;
                }
                startSlot = owner.mainWeapon.weaponInfo.startSlot;
                endSlot = owner.mainWeapon.weaponInfo.endSlot;
                owner.mainWeapon.weaponInfo.weaponVFX.SetTrail(true);
                owner.mainWeapon.weaponInfo.weaponSFX.PlayAttackSFX(owner, 1.0f);
                break;
        
            case EArmedType.Second:
                if (owner.secondWeapon == null)
                {
                    Debug.LogError("Second Weapon is null");
                    return;
                }
                startSlot = owner.secondWeapon.weaponInfo.startSlot;
                endSlot = owner.secondWeapon.weaponInfo.endSlot;
                owner.secondWeapon.weaponInfo.weaponVFX.SetTrail(true);
                owner.secondWeapon.weaponInfo.weaponSFX.PlayAttackSFX(owner, 1.0f);
                break;
        }

        isTrace = true;
        hitInfo.hitCharacters.Clear();
    }

    public void OnTraceEnd()
    {
        isTrace = false;
        hitInfo.hitCharacters.Clear();

        if (owner.mainWeapon != null)
        {
            owner.mainWeapon.WeaponReset();
        }
        if (owner.secondWeapon != null)
        {
            owner.secondWeapon.WeaponReset();
        }
    }

    #endregion

    #region Hit Reaction Component

    public void PlayHitStop()
    {
        //float stopTime = owner.hitReactionComponent.GetHitReaction().HitFeedback.stopTime;
        //float stopScale = owner.hitReactionComponent.GetHitReaction().HitFeedback.stopScale;
        owner.hitReactionComponent.SetHitStop(0.05f, 0.08f);
    }

    private void PlayCameraShake()
    {
        //owner.hitReactionComponent.PlayCameraShake();
    }

    private void PlayHitFeedback()
    {
        if (owner.hitReactionComponent.GetHitReaction().HitFeedback.isHitStop)
        {
            PlayHitStop();
        }

        if (owner.hitReactionComponent.GetHitReaction().HitFeedback.isShake)
        {
            PlayCameraShake();
        }
    }

    private void PlayKnockback(CharacterActor hitActor)
    {
        hitActor.hitReactionComponent.SetKnockback(owner.attackComponent.CurrentSkillData.hitReactionDatas[owner.attackComponent.CurrentSkillIndex].hitReactionData[0].HitKnockback, owner);
    }

    private void PlayAirborne(CharacterActor hitActor)
    {
        hitActor.hitReactionComponent.SetAirborne(owner.attackComponent.CurrentSkillData.hitReactionDatas[owner.attackComponent.CurrentSkillIndex].hitReactionData[0].HitAirborne, hitActor);
    }

    private void PlayHitVFX()
    {
        if (owner.hitReactionComponent.GetHitReaction().HitParticleFX.hitParticle != null)
        {
            Vector3 spawnPosition = hitInfo.hitPoint;
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(owner.hitReactionComponent.GetHitReaction().HitParticleFX.eulerOffset);
            ParticleSystem effect = Instantiate(owner.hitReactionComponent.GetHitReaction().HitParticleFX.hitParticle, spawnPosition, spawnRotation);
            if (owner.hitReactionComponent.GetHitReaction().HitParticleFX.isParent)
            {
                effect.transform.SetParent(hitInfo.hitCharacter.transform);
            }
            effect.transform.localScale = owner.hitReactionComponent.GetHitReaction().HitParticleFX.scale;
            Destroy(effect.gameObject, effect.main.startLifetime.constantMax);
        }
    }

    private void PlayHitSFX(CharacterActor hitActor)
    {
        //owner.hitReactionComponent.GetHitReaction().HitSoundFX.Play(hitActor.characterAudio, owner.hitReactionComponent.GetHitReaction().HitSoundFX.hitClips);
    }

    #endregion

    public void HitReaction(CharacterActor hitActor)
    {
        switch (traceInfo.armedType)
        {
            case EArmedType.Main:
                if (owner.mainWeapon != null)
                {
                    owner.mainWeapon.weaponInfo.weaponVFX.PlayHitParticle();
                }
                break;
        
            case EArmedType.Second:
                if (owner.secondWeapon != null)
                {
                    owner.secondWeapon.weaponInfo.weaponVFX.PlayHitParticle();
                }
                break;
        }

        owner.hitReactionComponent.HitAnimation(hitActor);
        PlayHitFeedback();
        PlayKnockback(hitActor);
        PlayAirborne(hitActor);
        PlayHitVFX();
        PlayHitSFX(hitActor);
    }

    public void HitEvent(Character hit, Vector3 hitPoint, Vector3 hitDirection)
    {
        CharacterActor hitActor = hit as CharacterActor;
        DamageInfo damageInfo = new DamageInfo(owner, owner.hitReactionComponent.GetHitReaction().Damage, OnReceiveDamage, hitPoint);

        if (hitActor == null)
        {
            var damagable = hit as IDamagable;
            if (damagable != null)
            {
                hit.TakeDamage(damageInfo, hitDirection);
            }
            return;
        }

        if (hitActor.statsComponent.isInvincible) return;

        var damageable = hitActor as IDamagable;
        if (damageable != null)
        {
            hitInfo.hitPoint = hitActor.animator.GetBoneTransform(HumanBodyBones.Chest).position;
            hitInfo.hitCharacter = hitActor;
            hitActor.DoLookOpposite(owner.transform.position, 0.5f);

            switch (hitActor.hitReactionComponent.combatData.combatType)
            {
                case ECombatType.None:
                case ECombatType.Attack:
                case ECombatType.HitReaction:
                    HitReaction(hitActor);
                    break;

                case ECombatType.Dodge:
                    owner.hitReactionComponent.SetHitStop(0.35f, 0.0f);
                    break;
            }
            damageable.TakeDamage(damageInfo, hitDirection);
        }
    }

    public HitInfo GetHitInfo()
    {
        return hitInfo;
    }
}
