using UnityEngine;
public static class AnimatorHelper
{
    public static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    public static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    public static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    public static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    public static readonly int JumpCountHash = Animator.StringToHash("jumpCount");
    public static readonly int IsFallingHash = Animator.StringToHash("isFalling");
    public static readonly int IsWallSlidingHash = Animator.StringToHash("isWallSliding");
}
