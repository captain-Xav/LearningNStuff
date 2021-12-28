using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float _currentVelocityY;

    public PlayerJumpState(PlayerStateMachine ctx, PlayerStateFactory factory)
        : base(ctx, factory, isRootState: true)
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
        this.HandleJump();
    }

    public override void ExitState()
    {
        this.Ctx.Animator.SetBool(PlayerStateMachine.IsJumpingHash, false);
        this.Ctx.CurrentJumpCountResetRoutine = this.Ctx.StartCoroutine(this.JumpCountResetRoutine());
        if (this.Ctx.JumpCount == this.Ctx.MaxJumpCount)
        {
            this.Ctx.JumpCount = 0;
            this.Ctx.Animator.SetInteger(PlayerStateMachine.JumpCountHash, 0);
        }
    }

    public override void InitializeSubState()
    {
        this.SetSubState(this.Factory.MidAir());
    }

    public override void UpdateState()
    {
        this.HandleGravity();

        this.CheckSwitchStates();
    }

    void HandleJump()
    {
        if (this.Ctx.JumpCount < this.Ctx.MaxJumpCount && this.Ctx.CurrentJumpCountResetRoutine != null)
        {
            this.Ctx.StopCoroutine(this.Ctx.CurrentJumpCountResetRoutine);
        }

        this.Ctx.Animator.SetBool(PlayerStateMachine.IsJumpingHash, true);
        this.Ctx.JumpCount += 1;
        this.Ctx.Animator.SetInteger(PlayerStateMachine.JumpCountHash, this.Ctx.JumpCount);
        _currentVelocityY = this.Ctx.InitialjumpVelocities[this.Ctx.JumpCount - 1];
        this.Ctx.AppliedMovementY = this.Ctx.InitialjumpVelocities[this.Ctx.JumpCount - 1];
    }

    void HandleGravity()
    {
        float fallMutiplier = this.Ctx.AppliedMovementY <= 0.0f || !this.Ctx.IsJumpPressed ? 2.0f : 1.0f;
        float previousYVelocity = _currentVelocityY;
        _currentVelocityY = _currentVelocityY + (this.Ctx.JumpGravities[this.Ctx.JumpCount] * fallMutiplier * Time.deltaTime);
        this.Ctx.AppliedMovementY = (previousYVelocity + _currentVelocityY) * .5f;
    }

    IEnumerator JumpCountResetRoutine()
    {
        yield return new WaitForSeconds(.25f);
        this.Ctx.JumpCount = 0;
    }
}
