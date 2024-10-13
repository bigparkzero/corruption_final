using UnityEngine;

[RequireComponent(typeof(AISenseDetection), typeof(AISenseHearing))]
public class AIPerception : MonoBehaviour
{
    [Header("[Component]")]
    public CharacterActor owner;

    [Header("[AI Perception]")]
    public AISenseDetection senseDectection;
    public AISenseHearing senseHearing;

    private void Awake()
    {
        owner = GetComponent<CharacterActor>();
        senseDectection = GetComponent<AISenseDetection>();
        senseHearing = GetComponent<AISenseHearing>();
    }
}