using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELocomotionState
{
    Stop,
    Walk,
    Sprint,
}

public enum EMovementTypeState
{
    None,
    Grounded,
    InAir,
}

public enum EBodySocket
{
    LeftHand,
    RightHand,
    Core,
}

public abstract class CharacterBase : MonoBehaviour
{
    [Header("[Component]")]
    [HideInInspector] public IAnimationComponent animationComponent;

    [Space]
    [Header("[Death Option]")]
    public bool useRagdoll;

    protected virtual void Awake()
    {
        IAnimationComponent animComponent = GetComponent<IAnimationComponent>();
    }

    protected virtual void Start() { }

    protected virtual void Update() { }

    protected abstract void ApplyGravity();

    protected abstract void CheckGround();

    protected abstract void AirControl();

    public abstract void StopEveryCoroutines();
}
