using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : SubState<PlayerContext, PlayerStateFactory>
{
    public PlayerIdleState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    public override void CheckSwitchStates()
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

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWalkingHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsRunningHash, false);
        this.Ctx.AppliedMovementXZ = Vector3.zero;
    }
}
