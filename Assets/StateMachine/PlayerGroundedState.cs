using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    float _isFallingTimer = 0f;

    public PlayerGroundedState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory, isRootState: true)
    {
        this.InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        // If player is grounded and jump is pressed, switch to jump state
        if (this.Ctx.IsJumpPressed && Time.time - 0.2f < this.Ctx.JumpPressTimer)
        {
            this.SwitchState(this.Factory.Jump());
        }
        else if (!this.Ctx.CharacterController.isGrounded && _isFallingTimer > 0.15f)
        {
            this.SwitchState(this.Factory.Fall());
        }
    }

    public override void EnterState()
    {
        // this.Ctx.CurrentMovementY = this.Ctx.GroundedGravity;
        this.Ctx.AppliedMovementY = this.Ctx.FallingGravity;
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if (!this.Ctx.IsMovementPressed)
        {
            this.SetSubState(this.Factory.Idle());
        }
        else if (this.Ctx.IsMovementPressed && !this.Ctx.IsRunPressed)
        {
            this.SetSubState(this.Factory.Walk());
        }
        else
        {
            this.SetSubState(this.Factory.Run());
        }
    }

    public override void UpdateState()
    {
        _isFallingTimer = this.Ctx.CharacterController.isGrounded ? 0 : _isFallingTimer + Time.deltaTime;

        this.Ctx.AppliedMovementY = this.Ctx.GroundedGravity;

        if (this.Ctx.IsMovementPressed)
        {
            Ray rayDown = new Ray(this.Ctx.CharacterController.transform.position, Vector3.down);

            Physics.Raycast(rayDown, out RaycastHit hitDownInfo);

            if (hitDownInfo.normal.y < 1)
            {
                this.Ctx.AppliedMovementY = (2 - hitDownInfo.normal.normalized.y) * this.Ctx.FallingGravity;
            }
        }

        this.CheckSwitchStates();
    }
}
