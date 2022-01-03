using UnityEngine;

public class PlayerFallState : BaseState<PlayerContext, PlayerStateFactory>
{
    float _gravity;

    public PlayerFallState(PlayerContext ctx, PlayerStateFactory factory)
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
        this.Ctx.Animator.SetBool(AnimatorHelper.IsFallingHash, true);
        _gravity = this.Ctx.FallingGravity;
    }

    protected override void OnExitState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsFallingHash, false);
    }

    protected override void OnInitializeSubState()
    {
        this.SetSubState(this.Factory.GetState(PlayerState.MidAir));
    }

    protected override void OnUpdateState()
    {
        this.Ctx.AppliedMovementY = _gravity *= 1.10f;
    }
}
