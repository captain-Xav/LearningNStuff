using UnityEngine;

public class PlayerFallState : SuperState<PlayerContext, PlayerStateFactory>
{
    float _gravity;

    public PlayerFallState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    public override void CheckSwitchStates()
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

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsFallingHash, true);
        _gravity = this.Ctx.FallingGravity;
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();

        this.Ctx.Animator.SetBool(AnimatorHelper.IsFallingHash, false);
    }

    public override void InitializeSubState()
    {
        this.SetSubState(this.Factory.GetState(PlayerState.MidAir));
    }

    public override void UpdateState()
    {
        this.Ctx.AppliedMovementY = _gravity *= 1.10f;
        base.UpdateState();
    }
}
