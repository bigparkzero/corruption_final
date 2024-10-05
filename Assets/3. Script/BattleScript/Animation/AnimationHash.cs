using UnityEngine;

public static class AnimationHash
{
    // Float Parameter
    public static readonly int HASH_LOCOMOTION_SPEED = Animator.StringToHash("Locomotion Speed");
    public static readonly int HASH_INPUT_DIRECTION = Animator.StringToHash("Input Direction");

    // Bool Parameter
    public static readonly int HASH_GROUNDED = Animator.StringToHash("Grounded");
    public static readonly int HASH_IS_JUMP = Animator.StringToHash("IsJump");

    // State Name
    public static readonly int HASH_DEAD = Animator.StringToHash("Dead");
    public static readonly int HASH_JUMP = Animator.StringToHash("Jump");
    public static readonly int HASH_LANDING = Animator.StringToHash("Landing");
    public static readonly int HASH_ROLL = Animator.StringToHash("Roll");
    public static readonly int HASH_DASH = Animator.StringToHash("Dash");
}
