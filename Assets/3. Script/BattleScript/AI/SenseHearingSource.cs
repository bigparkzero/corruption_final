using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NoiseInfo
{
    [Header("[Noise Info]")]
    public LayerMask layerMask;
    public Collider[] colliders;
    public float noiseInpulse;
}

public class SenseHearingSource : MonoBehaviour
{
    [Header("[Component]")]
    [SerializeField] private Rigidbody sourceRig;
    [SerializeField] private Collider sourceCollider;

    [Header("[Sense Hearing Source]")]
    [SerializeField] private NoiseInfo noiseInfo;
    [SerializeField] private float goalRange = 20.0f;
    [SerializeField] private float noiseRange = 0.0f;
    [SerializeField] private float duration = 1.0f;

    [Header("[Coroutine]")]
    private Coroutine C_Noise;

    private void Start()
    {
        sourceRig = GetComponent<Rigidbody>();
        sourceCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > noiseInfo.noiseInpulse)
        {
            SetNoise();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, noiseRange);
    }

    private IEnumerator Noise()
    {
        float elapsedTime = 0.0f;
        noiseRange = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            noiseRange = Mathf.Lerp(0.0f, goalRange, elapsedTime / duration);
            noiseInfo.colliders = Physics.OverlapSphere(transform.position, noiseRange, noiseInfo.layerMask.value);

            foreach (Collider noise in noiseInfo.colliders)
            {
                AISenseHearing senseHearing = noise.GetComponent<AISenseHearing>();
                if (senseHearing != null)
                {
                    senseHearing.ReceiveNoise(this);
                }
            }

            yield return null;
        }
    }

    public void SetNoise()
    {
        if (C_Noise != null) StopCoroutine(C_Noise);
        C_Noise = StartCoroutine(Noise());
    }
}