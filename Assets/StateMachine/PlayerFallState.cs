using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerFallState : PlayerBaseState
{
    float _gravity;

    public PlayerFallState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory, true)
    {
        this.InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        if (this.Ctx.CharacterController.isGrounded)
        {
            this.SwitchState(this.Factory.Grounded());
        }
    }

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsFallingHash, true);
        _gravity = this.Ctx.FallingGravity;
    }

    public override void ExitState()
    {
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsFallingHash, false);
    }

    public override void InitializeSubState()
    {
        this.SetSubState(this.Factory.MidAir());
    }

    public override void UpdateState()
    {
        this.Ctx.AppliedMovementY = _gravity *= 1.10f;

        this.CheckSwitchStates();
    }
}
