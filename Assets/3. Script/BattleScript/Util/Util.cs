using System.Collections;
using UnityEngine;

public static class Util
{
    public static T GetNearestToComponent<T>(CharacterActor owner, float radius, LayerMask layerMask) where T : Component
    {
        var colls = Physics.OverlapSphere(owner.transform.position, radius, layerMask.value);
        float nearestToDistance = Mathf.Infinity;
        T target = null;

        foreach (var coll in colls)
        {
            if (owner.gameObject != coll.gameObject && coll.GetComponentInParent<T>())
            {
                float distance = GetDistance(owner.transform.position, coll.transform.position);
                if (nearestToDistance > distance)
                {
                    nearestToDistance = distance;
                    target = coll.GetComponentInParent<T>();
                }
            }
        }

        return target;
    }

    public static T GetDirectionToComponent<T>(CharacterActor owner, float radius, float angle, LayerMask layerMask) where T : Component
    {
        var colls = Physics.OverlapSphere(owner.transform.position, radius, layerMask.value);
        float nearestToDistance = Mathf.Infinity;
        T target = null;

        foreach (var coll in colls)
        {
            Vector3 direction = GetDirection(owner.transform.position, coll.transform.position);
            if (Vector3.Angle(owner.GetComponent<CharacterMovement>().GetInputDirection, direction) < angle * 0.5f)
            {
                if (owner.gameObject != coll.gameObject)
                {
                    float distance = GetDistance(owner.transform.position, coll.transform.position, true);

                    if (nearestToDistance > distance)
                    {
                        nearestToDistance = distance;

                        target = coll.GetComponentInParent<T>();
                    }
                }
            }
        }

        return target;
    }

    public static Vector3 GetDirection(Vector3 to, Vector3 from, bool ignoreY = true)
    {
        if (ignoreY)
        {
            to.y = 0.0f;
            from.y = 0.0f;
        }
        Vector3 direction = (from - to).normalized;

        return direction;
    }

    public static Vector3 GetCenterPosition(Vector3 a, Vector3 b)
    {
        return (a + b) * 0.5f;
    }

    public static float GetDistance(CharacterActor from, CharacterActor to, bool ignoreY = true)
    {
        Vector3 fromPos = from.transform.position;
        Vector3 toPos = to.transform.position;

        if (ignoreY)
        {
            fromPos.y = 0;
            toPos.y = 0;
        }

        return Vector3.Distance(fromPos, toPos);
    }

    public static float GetDistance(Transform from, Transform to, bool ignoreY = true)
    {
        Vector3 fromPos = from.position;
        Vector3 toPos = to.position;

        if (ignoreY)
        {
            fromPos.y = 0;
            toPos.y = 0;
        }

        return Vector3.Distance(fromPos, toPos);
    }

    public static float GetDistance(Vector3 from, Vector3 to, bool ignoreY = true)
    {
        if (ignoreY)
        {
            from.y = 0;
            to.y = 0;
        }

        return Vector3.Distance(from, to);
    }

    public static float GetHeight(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.y - b.y);
    }

    public static IEnumerator DistanceMatching(CharacterActor owner, Vector3 startPos, Vector3 endPos, float duration, bool isRelative = false, bool ignoreY = false)
    {
        float currentTime = 0f;
        Vector3 direction = GetDirection(startPos, endPos, false);

        if (ignoreY)
        {
            startPos.y = owner.transform.position.y;
            endPos.y = owner.transform.position.y;
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            float ratio = currentTime / duration;

            if (Physics.SphereCast(owner.transform.position + owner.transform.TransformDirection(0f, 1f, 0f),
                owner.characterController.radius, direction, out RaycastHit hit,
                owner.characterController.radius, owner.locomotionData.groundLayer.value))
            {
                if (hit.collider)
                {
                    yield break;
                }
            }

            if (isRelative)
            {
                owner.transform.localPosition = Vector3.Lerp(startPos, endPos, ratio);
            }
            else
            {
                owner.transform.position = Vector3.Lerp(startPos, endPos, ratio);
            }

            yield return null;
        }
    }

    public static int GetAnimEventCount(AnimationClip clip, string eventName)
    {
        if (clip == null) return 0;
        int count = 0;

        foreach (var animEvent in clip.events)
        {
            if (animEvent.functionName == eventName)
            {
                count++;
            }
        }

        return count;
    }
}
