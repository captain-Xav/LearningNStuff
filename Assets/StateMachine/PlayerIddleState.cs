using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIddleState : SubState<PlayerContext, PlayerStateFactory>
{
    public PlayerIddleState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    public override void CheckSwitchStates()
    {
        if (this.Ctx.IsMovementPressed && this.Ctx.IsRunPressed)
        {
            this.SwitchState(this.Factory.Run());
        }
        else if (this.Ctx.IsMovementPressed)
        {
            this.SwitchState(this.Factory.Walk());
        }
    }

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWalkingHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsRunningHash, false);
        this.Ctx.AppliedMovementXZ = Vector3.zero;
    }
}
