using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DodgeComponent : MonoBehaviour
{
    CharacterActor owner;

    float duration = 0.25f;
    float moveDistance = 5.0f;
    float stoppingDistance = 1.5f;

    UnityAction beginEvent;
    UnityAction endEvent;

    [HideInInspector] public Coroutine C_Dodge;
    [HideInInspector] public Coroutine C_DistanceMatching;

    private void Start()
    {
        owner = GetComponent<CharacterActor>();

        beginEvent += InvincibilityEnable;
        endEvent += InvincibilityDisable;
    }

    IEnumerator Roll(Vector3 direction)
    {
        if (direction == Vector3.zero) direction = transform.forward;

        Vector3 moveDirection = direction * moveDistance;
        Vector3 offset = transform.TransformDirection(0, 0, moveDistance);
        if (direction != Vector3.zero) offset = Vector3.zero;
        owner.animator.SetFloat(AnimationHash.HASH_INPUT_DIRECTION, Vector3.SignedAngle(transform.forward, direction, Vector3.up));

        owner.animator.CrossFadeInFixedTime(AnimationHash.HASH_ROLL, 0.1f);
        moveDirection += offset;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + moveDirection;
        if (C_DistanceMatching != null) StopCoroutine(C_DistanceMatching);
        C_DistanceMatching = StartCoroutine(Util.DistanceMatching(owner, startPosition, endPosition, duration));

        beginEvent?.Invoke();
        yield return new WaitForSeconds(duration);
        endEvent?.Invoke();
    }

    IEnumerator Dash(Vector3 direction)
    {
        if (direction == Vector3.zero) direction = transform.forward;

        Vector3 moveDirection = direction * moveDistance;
        Vector3 offset = transform.TransformDirection(0, 0, moveDistance);
        if (direction != Vector3.zero) offset = Vector3.zero;
        owner.animator.SetFloat(AnimationHash.HASH_INPUT_DIRECTION, Vector3.SignedAngle(transform.forward, direction, Vector3.up));

        owner.animator.CrossFadeInFixedTime(AnimationHash.HASH_DASH, 0.1f);
        moveDirection += offset;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + moveDirection;
        if (C_DistanceMatching != null) StopCoroutine(C_DistanceMatching);
        C_DistanceMatching = StartCoroutine(Util.DistanceMatching(owner, startPosition, endPosition, duration));

        beginEvent?.Invoke();
        yield return new WaitForSeconds(duration);
        endEvent?.Invoke();
    }

    public void DoRoll(Vector3 direction)
    {
        if (C_Dodge != null)
        {
            StopCoroutine(C_Dodge);
            endEvent?.Invoke();
        }
        C_Dodge = StartCoroutine(Roll(direction));
    }

    public void DoDash(Vector3 direction)
    {
        if (C_Dodge != null)
        {
            StopCoroutine(C_Dodge);
            endEvent?.Invoke();
        }
        C_Dodge = StartCoroutine(Dash(direction));
    }

    public float GetDuration()
    {
        return duration;
    }

    public void InvincibilityEnable()
    {
        owner.statsComponent.isInvincible = true;
    }

    public void InvincibilityDisable()
    {
        owner.statsComponent.isInvincible = false;
    }
}
