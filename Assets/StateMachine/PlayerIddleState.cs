using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIddleState : PlayerBaseState
{
    public PlayerIddleState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory)
    {
        this.EnterState();
    }

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
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsWalkingHash, false);
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsRunningHash, false);

        this.Ctx.AppliedMovementXZ = Vector3.zero;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        this.CheckSwitchStates();
    }
}
