using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    bool _isFalling = false;
    float _jumpBuffer = 0f;

    public PlayerGroundedState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory, isRootState: true)
    {
        this.InitializeSubState();
    }

    public override void CheckSwitchStates()
    {
        // If player is grounded and jump is pressed, switch to jump state
        if (this.Ctx.IsJumpPressed && Time.time - 0.1f < this.Ctx.JumpPressTimer)
        {
            this.SwitchState(this.Factory.Jump());
        }
        else if (_isFalling && _jumpBuffer > 0.15f)
        {
            this.SwitchState(this.Factory.Fall());
        }
    }

    public override void EnterState()
    {
        // this.Ctx.CurrentMovementY = this.Ctx.GroundedGravity;
        this.Ctx.AppliedMovementY = this.Ctx.GroundedGravity;
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
        if (this.Ctx.CharacterController.isGrounded)
        {
            _jumpBuffer = 0f;
            _isFalling = false;
        }
        else if (this.Ctx.IsMovementPressed && !this.Ctx.CharacterController.isGrounded)
        {
            Vector3 position = this.Ctx.CharacterController.transform.position;

            Ray rayDown = new Ray(position, Vector3.down);

            Physics.Raycast(rayDown, out RaycastHit hitDownInfo);

            if (hitDownInfo.distance < 2f)
            {
                position.y = hitDownInfo.point.y + (0.5f * Vector3.Angle(Vector3.up, hitDownInfo.normal) / 100f) ;

                this.Ctx.CharacterController.transform.position = position;
            }
            else
            {
                _jumpBuffer += Time.deltaTime;
                _isFalling = true;
            }
        }
        else
        {
            _jumpBuffer += Time.deltaTime;
        }

        this.CheckSwitchStates();
    }
}
