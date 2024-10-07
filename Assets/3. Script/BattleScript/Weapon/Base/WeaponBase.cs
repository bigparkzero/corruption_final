using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EMorphType
{
    None,
    Morphed,
}

[Serializable]
public struct WeaponVFX
{
    public GameObject trail;
    public ParticleSystem hitFX;

    public void SetTrail(bool isActive)
    {
        if (trail == null) return;

        trail.SetActive(isActive);
    }

    public void PlayHitParticle()
    {
        if (hitFX == null) return;

        hitFX.Play();
    }
}

[Serializable]
public struct WeaponSFX
{
    public List<AudioClip> attackClips;
    public List<AudioClip> hitClips;

    public void PlayAttackSFX(Character owner, float volume = 1f)
    {
        if (owner.characterAudio == null || attackClips.Count <= 0) return;

        owner.characterAudio.PlayOneShot(attackClips[UnityEngine.Random.Range(0, attackClips.Count)], volume);
    }

    public void PlayHitSFX(Character owner, float volume = 1f)
    {
        if (owner.characterAudio == null || hitClips.Count <= 0) return;

        owner.characterAudio.PlayOneShot(hitClips[UnityEngine.Random.Range(0, hitClips.Count)], volume);
    }
}

[Serializable]
public struct WeaponInfo
{
    public EMorphType morphType;
    public EBodySocket bodySocket;
    public Character owner;
    public Transform startSlot;
    public Transform endSlot;
    public WeaponVFX weaponVFX;
    public WeaponSFX weaponSFX;
}

public class WeaponBase : MonoBehaviour
{
    [Header("[Weapon Base]")]
    public WeaponInfo weaponInfo;

    public void WeaponReset()
    {
        weaponInfo.weaponVFX.SetTrail(false);
    }
}
