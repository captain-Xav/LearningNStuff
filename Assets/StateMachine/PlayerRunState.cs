using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory)
    {
        this.EnterState();
    }

    public override void CheckSwitchStates()
    {
        if (!this.Ctx.IsMovementPressed)
        {
            this.SwitchState(this.Factory.Idle());
        }
        else if (this.Ctx.IsMovementPressed && !this.Ctx.IsRunPressed)
        {
            this.SwitchState(this.Factory.Walk());
        }
    }

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsWalkingHash, true);
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsRunningHash, true);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        this.Ctx.AppliedMovementXZ = this.Ctx.CurrentMovement * this.Ctx.RunSpeed;
        
        this.CheckSwitchStates();
    }
}
