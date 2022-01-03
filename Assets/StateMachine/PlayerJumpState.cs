using System.Collections;
using UnityEngine;

public class PlayerJumpState : BaseState<PlayerContext, PlayerStateFactory>
{
    private float _currentVelocityY;
    private int JumpCount { get; set; }
    private Coroutine CurrentJumpCountResetRoutine { get; set; } = null;

    public PlayerJumpState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    protected override void OnCheckSwitchState()
    {
        if (this.Ctx.CharacterPhysics.IsGrounded)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Grounded));
        }
        else if (PlayerWallSlideState.IsWallSliding(this.Ctx.CharacterController, out _))
        {
            this.SwitchState(this.Factory.GetState(PlayerState.WallSlide));
        }
    }

    protected override void OnEnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsJumpingHash, true);
        this.HandleJump();
    }

    protected override void OnExitState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsJumpingHash, false);
        this.CurrentJumpCountResetRoutine = this.Ctx.StartCoroutine(this.JumpCountResetRoutine());
        if (this.JumpCount == this.Ctx.MaxJumpCount)
        {
            this.JumpCount = 0;
            this.Ctx.Animator.SetInteger(AnimatorHelper.JumpCountHash, 0);
        }
    }

    protected override void OnInitializeSubState()
    {
        this.SetSubState(this.Factory.GetState(PlayerState.MidAir));
    }

    protected override void OnUpdateState()
    {
        this.HandleGravity();
    }

    void HandleJump()
    {
        if (this.JumpCount < this.Ctx.MaxJumpCount && this.CurrentJumpCountResetRoutine != null)
        {
            this.Ctx.StopCoroutine(this.CurrentJumpCountResetRoutine);
        }

        this.JumpCount += 1;
        this.Ctx.Animator.SetInteger(AnimatorHelper.JumpCountHash, this.JumpCount);
        _currentVelocityY = this.Ctx.InitialjumpVelocities[this.JumpCount - 1];
        this.Ctx.AppliedMovementY = this.Ctx.InitialjumpVelocities[this.JumpCount - 1];
    }

    void HandleGravity()
    {
        float fallMutiplier = this.Ctx.AppliedMovementY <= 0.0f || !this.Ctx.IsJumpPressed ? 2.0f : 1.0f;
        float previousYVelocity = _currentVelocityY;
        _currentVelocityY += (this.Ctx.JumpGravities[this.JumpCount] * fallMutiplier * Time.deltaTime);
        this.Ctx.AppliedMovementY = (previousYVelocity + _currentVelocityY) * .5f;
    }

    IEnumerator JumpCountResetRoutine()
    {
        yield return new WaitForSeconds(.25f);
        this.JumpCount = 0;
    }
}
