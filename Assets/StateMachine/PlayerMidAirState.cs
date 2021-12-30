using UnityEngine;

public class PlayerMidAirState : SubState<PlayerContext, PlayerStateFactory>
{
    float _midAirSpeed;
    public PlayerMidAirState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

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

    public override void UpdateState()
    {
        if (this.Ctx.IsMovementPressed)
            this.Ctx.AppliedMovementXZ = this.Ctx.CurrentMovement * _midAirSpeed;

        base.UpdateState();
    }
}
