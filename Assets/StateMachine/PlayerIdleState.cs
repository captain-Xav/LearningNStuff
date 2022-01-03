using UnityEngine;

public class PlayerIdleState : BaseState<PlayerContext, PlayerStateFactory>
{
    public PlayerIdleState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    protected override void OnCheckSwitchState()
    {
        if (this.Ctx.IsMovementPressed && this.Ctx.IsRunPressed)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Run));
        }
        else if (this.Ctx.IsMovementPressed)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Walk));
        }
    }

    protected override void OnEnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWalkingHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsRunningHash, false);
        this.Ctx.AppliedMovementXZ = Vector3.zero;
    }
}
