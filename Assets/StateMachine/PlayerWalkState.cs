using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    float _factor = 1f;
    float _timer = 0f;

    public PlayerWalkState(PlayerStateMachine ctx, PlayerStateFactory factory)
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
        else if (this.Ctx.IsMovementPressed && this.Ctx.IsRunPressed)
        {
            this.SwitchState(this.Factory.Run());
        }
    }

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsWalkingHash, true);
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsRunningHash, false);
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;

        if (_timer > 0.5f)
        {
            Vector3 position = this.Ctx.CharacterController.transform.position;
            Ray rayDown = new Ray(position, Vector3.down);
            Physics.Raycast(rayDown, out RaycastHit hitDownInfo);

            _factor = (180 - Vector3.Angle(this.Ctx.CharacterController.transform.forward, hitDownInfo.normal)) / 90;

            _timer = 0f;
        }

        this.Ctx.AppliedMovementXZ = this.Ctx.CurrentMovement * this.Ctx.WalkSpeed * _factor;

        this.CheckSwitchStates();
    }
}
