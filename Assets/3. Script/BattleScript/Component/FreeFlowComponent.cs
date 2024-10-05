using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FreeFlowComponent : MonoBehaviour
{
    [Header("[Component]")]
    private Character owner;

    [Header("[Freeflow Component]")]
    [SerializeField] private Character target;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float maxDistance = 5.0f;
    [SerializeField] private float minDistance = 0.0f;
    [SerializeField] private float stopInterval = 0.75f;
    [SerializeField] private float delayMoveTime = 0.02f;
    [Range(0.0f, 90.0f)][SerializeField] private float angle = 30.0f;
    [Range(0.1f, 1.0f)][SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float rotationDuration = 0.5f;
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private AnimationCurve attackCurve;
    [SerializeField] private float currentLerpTime = 0.0f;
    [Range(0.0f, 1.0f)][SerializeField] private float lerpTime = 1.0f;
    [Range(0.0f, 1.0f)][SerializeField] private float ratio = 1.0f;
    private Vector3 currentDirection;
    private float currentDistance;

    [Header("[Freeflow Event]")]
    [SerializeField] private UnityEvent beginEvent;
    [SerializeField] private UnityEvent endEvent;

    [Header("[Coroutine]")]
    private Coroutine C_Freeflow;
    private Coroutine C_DistanceMatching;

    [Header("[Debug]")]
    public bool isDrawGizmos = false;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        owner = GetComponent<Character>();
    }

    private IEnumerator Freeflow()
    {
        UpdateTarget();
        if (target == null) yield break;
        yield return new WaitForSeconds(delayMoveTime);

        currentDirection = Util.GetDirection(target.transform.position, owner.transform.position, true);
        if (C_DistanceMatching != null) StopCoroutine(C_DistanceMatching);
        C_DistanceMatching = StartCoroutine(Util.DistanceMatching(owner, owner.transform.position, target.transform.position + (currentDirection * minDistance), moveDuration, false, true));
        owner.DoLookOpposite(target.transform.position, rotationDuration);
        beginEvent?.Invoke();

        while (currentLerpTime < lerpTime && owner.hitReactionComponent.combatData.combatType != ECombatType.Dodge && !owner.locomotionData.isJump)
        {
            currentLerpTime += Time.deltaTime;
            currentDistance = Util.GetDistance(target.transform.position + (currentDirection * minDistance), owner.transform.position, true);

            if (currentDistance <= stopInterval)
            {
                FreeflowReset();
                yield break;
            }

            //ratio = attackCurve.Evaluate(owner.animator.GetFloat(AnimationHash.HASH_DISTANCE_MATCHING));
            //owner.animator.SetFloat(AnimationHash.HASH_ATTACK_RATIO, ratio);

            yield return null;
        }

        FreeflowReset();
    }

    public void OnFreeflow()
    {
        if (C_Freeflow != null) StopCoroutine(C_Freeflow);
        C_Freeflow = StartCoroutine(Freeflow());
    }

    public void StopFreeflow()
    {
        if (C_Freeflow != null) StopCoroutine(C_Freeflow);
        FreeflowReset();
    }

    public void UpdateTarget()
    {
        if (owner.locomotionData.hasInput)
        {
            target = Util.GetDirectionToComponent<Character>(owner, maxDistance, angle, targetLayer.value);

            // 키 입력 방향에 타겟이 없을 경우 가까운 적이 있는지 한번 더 체크
            if (target == null)
                target = Util.GetNearestToComponent<Character>(owner, maxDistance * 0.5f, targetLayer.value);
        }
        else
        {
            target = Util.GetNearestToComponent<Character>(owner, maxDistance * 0.5f, targetLayer.value);
        }

        currentLerpTime = 0.0f;

        // 타겟이 없으면 공격시 키 입력 방향으로 회전
        if (target == null)
        {
            if (owner.locomotionData.hasInput)
            {
                owner.DoLookOpposite(owner.transform.position + owner.characterMovement.GetInputDirection, 1.0f);
            }
            else
            {
                owner.DoLookOpposite(owner.transform.position + transform.forward, 1.0f);
            }
        }
        //else
        //{
        //    owner.targetingComponent.OnTargetChanged?.Invoke(target);
        //}
    }

    public void FreeflowReset()
    {
        if (C_DistanceMatching != null) StopCoroutine(C_DistanceMatching);
        ratio = 1.0f;
        //owner.animator.SetFloat(AnimationHash.HASH_ATTACK_RATIO, ratio);
        endEvent?.Invoke();
    }

    public Character GetTarget()
    {
        return target;
    }

    public float GetAngle()
    {
        return angle;
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }
}
