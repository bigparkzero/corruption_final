using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DetectionInfo
{
    [Header("[Detection Info]")]
    public float viewDistance;
    [Range(0.0f, 360.0f)] public float fieldOfViewAngle;
    public float viewHeight;

    public DetectionInfo(float distance, float angle, float height)
    {
        this.viewDistance = distance;
        this.fieldOfViewAngle = angle;
        this.viewHeight = height;
    }
}

public class AISenseDetection : MonoBehaviour
{
    private AIPerception aiPerception;

    [Header("[AI Sense - Dectection]")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private Character target;
    [SerializeField] private DetectionInfo detectionInfo;
    [SerializeField] private float intervalUpdate = 0.2f;
    [SerializeField] private bool targetInSight = false;

    [Header("[AI Sense - Event]")]
    public UnityAction<Character> OnSucceed;
    public UnityAction OnFailed;

    [Header("[AI Sense - Timer Handler]")]
    [SerializeField][Range(0.0f, 10.0f)] private float currentFailedTime = 0.0f;
    [SerializeField][Range(1.0f, 10.0f)] private float maxFailedTime = 2.0f;
    [SerializeField][Range(0.1f, 1.0f)] private float intensityTime = 0.1f;

    [Header("[AI Sense - Coroutine]")]
    private Coroutine C_Detection;

    [Header("[Draw Gizmos]")]
    [SerializeField] private bool isDrawGizmos;

    private void Start()
    {
        aiPerception = GetComponent<AIPerception>();

        OnDetection();
    }

    private void OnEnable()
    {
        OnSucceed += Succeed;
        OnFailed += Failed;
    }

    private void OnDisable()
    {
        OnSucceed -= Succeed;
        OnFailed -= Failed;
    }

    private IEnumerator Detection()
    {
        while (true)
        {
            CheckTargetInSight();

            yield return new WaitForSeconds(intervalUpdate);
        }
    }

    public void OnDetection()
    {
        if (C_Detection != null) StopCoroutine(C_Detection);
        C_Detection = StartCoroutine(Detection());
    }

    private void CheckTargetInSight()
    {
        CharacterPlayer player = Util.GetNearestToComponent<CharacterPlayer>(aiPerception.owner, detectionInfo.viewDistance, targetLayer);
        if (player == null)
        {
            targetInSight = false;
            OnFailed?.Invoke();
            return;
        }

        targetInSight = false;
        Vector3 directionToTarget = player.transform.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        if (distanceToTarget <= detectionInfo.viewDistance)
        {
            float height = Mathf.Abs(player.transform.position.y - transform.position.y);
            if (height <= detectionInfo.viewHeight)
            {
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                if (angleToTarget <= detectionInfo.fieldOfViewAngle * 0.5f)
                {
                    if (!Physics.Raycast(transform.position, directionToTarget.normalized, distanceToTarget, obstacleLayer.value))
                    {
                        targetInSight = true;
                        currentFailedTime = 0.0f;
                        OnSucceed?.Invoke(player);
                    }
                }
            }
        }

        if (!targetInSight)
        {
            OnFailed?.Invoke();
        }
    }

    private void Succeed(Character target)
    {
        if (target == null) return;

        if (this.target == null)
        {
            detectionInfo.viewDistance *= 2;
            detectionInfo.viewHeight *= 2;
            detectionInfo.fieldOfViewAngle = 360.0f;
        }

        this.target = target;
    }

    private void Failed()
    {
        if (target != null)
        {
            currentFailedTime += intensityTime;
            if (currentFailedTime > maxFailedTime)
            {
                detectionInfo.viewDistance *= 0.5f;
                detectionInfo.viewHeight *= 0.5f;
                detectionInfo.fieldOfViewAngle = 180.0f;

                target = null;
            }
        }
    }

    public void SetDetectionInfo(DetectionInfo info)
    {
        detectionInfo = info;
    }

    public DetectionInfo GetDetectionInfo()
    {
        return detectionInfo;
    }

    public Character GetTarget()
    {
        return target;
    }

    public float GetDistanceToTarget()
    {
        return target != null ? Util.GetDistance(transform.position, target.transform.position, true) : 0.0f;
    }
}