using UnityEngine;

public class MidAirState : PlayerBaseState
{
    float _midAirSpeed;
    Vector3 _midAirDirection;
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
        if (this.Ctx.IsMovementPressed && this.Ctx.IsRunPressed)
        {
            _midAirSpeed = this.Ctx.IsRunPressed ? this.Ctx.RunSpeed : this.Ctx.WalkSpeed;
        }
        else
        {
            _midAirSpeed = this.Ctx.MidAirSpeed;
        }

        _midAirDirection.x = this.Ctx.AppliedMovementXZ.x;
        _midAirDirection.z = this.Ctx.AppliedMovementXZ.z;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (this.Ctx.AppliedMovementXZ.magnitude < this.Ctx.MidAirSpeed || (this.Ctx.AppliedMovementXZ + this.Ctx.CurrentMovement).sqrMagnitude < this.Ctx.AppliedMovementXZ.sqrMagnitude)
        {
            this.Ctx.AppliedMovementXZ += this.Ctx.CurrentMovement;
        }

        Debug.Log($"this.Ctx.CurrentMovement: {this.Ctx.CurrentMovement}");
        Debug.Log($"this.Ctx.AppliedMovementXZ: {this.Ctx.AppliedMovementXZ}");
        Debug.Log($"this.Ctx.AppliedMovementXZ.magnitude: {this.Ctx.AppliedMovementXZ.magnitude}");
        Debug.Log($"(this.Ctx.AppliedMovementXZ + this.Ctx.CurrentMovement).magnitude: {(this.Ctx.AppliedMovementXZ + this.Ctx.CurrentMovement).magnitude}");

        this.CheckSwitchStates();
    }
}
