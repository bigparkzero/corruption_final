using Unity.Collections;
using UnityEngine;

[System.Serializable]
public struct CurrentMovementSettings
{
    public float walkSpeed;
    public float sprintSpeed;
    [ReadOnly] public float currentSpeed;
}

[CreateAssetMenu(fileName = "new LocomotionData", menuName = "Scriptable Object/Locomotion Data")]
public class LocomotionData : ScriptableObject
{
    [Header("[Locomotion States]")]
    public ELocomotionState locomotionState = ELocomotionState.Stop;
    public EMovementTypeState movementTypeState = EMovementTypeState.Grounded;

    [Header("[Essential Info]")]
    public Vector3 acceleration;
    public float speed;
    public float inputAmount;
    public float movementInputAmount;
    [Range(0.0f, 180.0f)] public float maxAngle = 90.0f;
    [Range(0.0f, 180.0f)] public float angle = 0.0f;
    [Range(0.0f, 180.0f)] public float direction = 0.0f;
    [Range(0.0f, 1.0f)] public float ratio = 1.0f;
    public bool isMoving = false;
    public bool hasInput;
    public bool hasMovementInput = false;

    [Header("[Caches Values]")]
    public Vector3 previousVelocity;

    [Header("[Grounded Values]")]
    public CurrentMovementSettings currentMovementSettings;
    [ReadOnly] public Vector3 velocity;
    public LayerMask groundLayer;
    public float gravity = Physics.gravity.y;
    public float maxAcceleration = 12f;
    public float maxDamp = 25f;
    public bool useGravity = true;
    [ReadOnly] public bool isGrounded = true;
    [ReadOnly] public bool isSprint = false;

    [Header("[Jump Values]")]
    public float maxJumpForce = 6f;
    [ReadOnly] public float jumpForce;
    public bool isJump = false;
    public AnimationCurve jumpCurve;

    [Header("[In Air Values]")]
    public float airControl = 0.2f;
    [ReadOnly] public float fallSpeed;
    [ReadOnly] public float fallingTime;
    public bool isInAir;

    [Header("[Rotation Values]")]
    public float rotationSpeed = 5f;

    protected void OnEnable()
    {
        OnReset();
    }

    public LocomotionData Clone()
    {
        LocomotionData data = CreateInstance<LocomotionData>();

        data.locomotionState = this.locomotionState;
        data.movementTypeState = this.movementTypeState;

        data.currentMovementSettings = this.currentMovementSettings;
        data.velocity = this.velocity;
        data.groundLayer = this.groundLayer;
        data.gravity = this.gravity;
        data.maxAcceleration = this.maxAcceleration;
        data.maxDamp = this.maxDamp;
        data.useGravity = this.useGravity;
        data.isGrounded = this.isGrounded;
        data.isSprint = this.isSprint;

        data.acceleration = this.acceleration;
        data.speed = this.speed;
        data.inputAmount = this.inputAmount;
        data.movementInputAmount = this.movementInputAmount;
        data.maxAngle = this.maxAngle;
        data.angle = this.angle;
        data.direction = this.direction;
        data.ratio = this.ratio;
        data.isMoving = this.isMoving;
        data.hasInput = this.hasInput;
        data.hasMovementInput = this.hasMovementInput;

        data.previousVelocity = this.previousVelocity;

        data.maxJumpForce = this.maxJumpForce;
        data.jumpForce = this.jumpForce;
        data.isJump = this.isJump;
        data.jumpCurve = this.jumpCurve;

        data.airControl = this.airControl;
        data.fallSpeed = this.fallSpeed;
        data.isInAir = this.isInAir;

        data.rotationSpeed = this.rotationSpeed;

        return data;
    }

    private void OnReset()
    {
        locomotionState = ELocomotionState.Stop;
        movementTypeState = EMovementTypeState.Grounded;

        velocity = Vector3.zero;
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        useGravity = true;
        isGrounded = true;
        isSprint = false;

        acceleration = Vector3.zero;
        speed = 0.0f;
        inputAmount = 0.0f;
        movementInputAmount = 0.0f;
        angle = 0.0f;
        direction = 0.0f;
        ratio = 1.0f;

        fallSpeed = 0.0f;
        velocity = Vector3.zero;
        previousVelocity = Vector3.zero;
        isJump = false;
        isInAir = false;
        isMoving = false;
        hasInput = false;
        hasMovementInput = false;

        previousVelocity = Vector3.zero;

        jumpForce = 0.0f;
        isJump = false;

        fallSpeed = 0.0f;
        fallingTime = 0.0f;
    }

    public Vector3 GetVelocity(CharacterActor character)
    {
        return character.characterController.velocity;
    }

    public Vector3 GetCurrentAcceleration()
    {
        return acceleration;
    }

    public float GetMaxAcceleration()
    {
        return maxAcceleration;
    }

    public float GetMaxDamp()
    {
        return maxDamp;
    }

    public Vector3 CalculateAcceleraction()
    {
        return (velocity - previousVelocity) / Time.deltaTime;
    }

    public Vector3 UnrotateVector(Vector3 vector, Quaternion rotation)
    {
        Quaternion inverseRotation = Quaternion.Inverse(rotation);
        return inverseRotation * vector;
    }

    public Vector3 GetControlRotation()
    {
        if (Camera.main != null)
        {
            return Camera.main.transform.eulerAngles;
        }
        else
        {
            Debug.LogError("Camera.main not found");
            return Vector3.zero;
        }
    }

    public float GetFloatValue(AnimationCurve curve, float inTime)
    {
        return curve.Evaluate(inTime);
    }
}
