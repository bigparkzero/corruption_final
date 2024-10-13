using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public enum EWallRideState
{
    None,
    Active,

}

public class WallRideComponent : MonoBehaviour
{
    [Header("[Component]")]
    public CharacterActor owner;

    [Header("[Wallride State Info]")]
    public EWallRideState wallRideState;
    public GameObject attachedWall = null;
    public Vector3 wallRideForward = Vector3.up;
    public Vector3 detachDirection;
    public bool isWallRiding;

    public LayerMask groundLayer;

    [Header("[Time]")]
    float DETACH_TIME = 0.1f;

    [Header("[Angle]")]
    public float MAX_TILT_ANGLE = 30f;
    public float TILT_SPEED = 5f;

    [Header("[Coroutine]")]
    Coroutine C_WallRide;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        owner = GetComponent<CharacterActor>();
        wallRideState = EWallRideState.None;
    }

    public void PlayWallRide(RaycastHit hit)
    {
        if (C_WallRide != null) StopCoroutine(C_WallRide);
        C_WallRide = StartCoroutine(WallRide(hit));
    }

    IEnumerator WallRide(RaycastHit hit)
    {
        Vector3 wallNormal = hit.normal;
        owner.animationEvent.Anim_GravityDisable();

        transform.position = hit.point + wallNormal * owner.characterController.radius;
        transform.rotation = Quaternion.LookRotation(-wallNormal, Vector3.up);

        SetWallRideInfo(hit);

        Quaternion originalRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(wallRideForward, wallNormal);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(transform.rotation);

        Vector3 limitedEuler = new Vector3(
            Mathf.Clamp(deltaRotation.eulerAngles.x, -MAX_TILT_ANGLE, MAX_TILT_ANGLE),
            transform.rotation.eulerAngles.y,
            Mathf.Clamp(deltaRotation.eulerAngles.z, -MAX_TILT_ANGLE, MAX_TILT_ANGLE)
        );
        Quaternion limitedRotation = Quaternion.Euler(limitedEuler);

        while (isWallRiding)
        {
            if (!PlayerInputManager.Instance.IsKeyUp(EKeyName.Move_Forward))
            {
                isWallRiding = false;

                break;
            }

            if (!CheckWall())
            {
                break;
            }

            Quaternion finalRotation = Quaternion.Slerp(transform.rotation, limitedRotation, Time.deltaTime * TILT_SPEED);
            transform.rotation = finalRotation;

            owner.characterController.Move(Vector3.up * owner.locomotionData.currentMovementSettings.sprintSpeed);

            if (PlayerInputManager.Instance.IsKeyStay(EKeyName.Move_Left))
            {
                owner.characterController.Move(-transform.right * owner.locomotionData.currentMovementSettings.walkSpeed);
            }
            if (PlayerInputManager.Instance.IsKeyStay(EKeyName.Move_Right))
            {
                owner.characterController.Move(transform.right * owner.locomotionData.currentMovementSettings.walkSpeed);
            }

            yield return null;
        }

        transform.rotation = originalRotation;
        owner.animationEvent.Anim_GravityEnable();

        wallRideState = EWallRideState.None;
        isWallRiding = false;

        if (isWallRiding)
        {
            // jump up
            owner.DoJump();
        }
    }

    void SetWallRideInfo(RaycastHit hit)
    {
        detachDirection = transform.forward;
        attachedWall = hit.collider.gameObject;

        wallRideState = EWallRideState.Active;
        isWallRiding = true;
    }

    bool CheckWall()
    {
        Vector3 feet = owner.transform.position;
        if (Physics.Raycast(feet, detachDirection, 0.2f, groundLayer, QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        return false;
    }
}
