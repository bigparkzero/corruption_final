using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class AISenseHearing : MonoBehaviour
{
    private AIPerception aiPerception;

    [Header("[AI Sense - Hearing]")]
    [SerializeField] private SenseHearingSource senseHearingSource;
    [SerializeField] private float range = 20.0f;
    [SerializeField] UnityEvent OnHearingEvent;

    [Header("[Draw Gizmos]")]
    [SerializeField] private bool isDrawGizmos;

    private void Start()
    {
        aiPerception = GetComponent<AIPerception>();
    }

    public void ReceiveNoise(SenseHearingSource source)
    {
        senseHearingSource = source;
        OnHearingEvent?.Invoke();
    }

    public SenseHearingSource GetHearingSource()
    {
        return senseHearingSource;
    }
}