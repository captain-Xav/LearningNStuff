using UnityEngine;

public class MidAirState : PlayerBaseState
{
    float _midAirSpeed;
    public MidAirState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory)
    {
        this.EnterState();
    }

    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        if (this.Ctx.IsMovementPressed)
        {
            _midAirSpeed = this.Ctx.IsRunPressed ? this.Ctx.RunSpeed : this.Ctx.WalkSpeed;
        }
        else
        {
            _midAirSpeed = 1f;
        }
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        this.Ctx.AppliedMovementXZ = this.Ctx.CurrentMovement * _midAirSpeed;

        this.CheckSwitchStates();
    }
}
