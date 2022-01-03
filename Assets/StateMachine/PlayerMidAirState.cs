using UnityEngine;

public class PlayerMidAirState : BaseState<PlayerContext, PlayerStateFactory>
{
    float _midAirSpeed;
    public PlayerMidAirState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    protected override void OnEnterState()
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

    protected override void OnUpdateState()
    {
        if (this.Ctx.IsMovementPressed)
        {
            this.Ctx.AppliedMovementXZ = this.Ctx.CurrentMovement * _midAirSpeed;
        }
    }
}
