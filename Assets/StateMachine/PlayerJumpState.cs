using System.Collections;
using UnityEngine;

public class PlayerJumpState : SuperState<PlayerContext, PlayerStateFactory>
{
    private float _currentVelocityY;

    public PlayerJumpState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    public override void CheckSwitchStates()
    {
        if (this.Ctx.CharacterController.isGrounded)
        {
            this.SwitchState(this.Factory.Grounded());
        }
        else if (this.CheckWallSlideCondition())
        {
            this.SwitchState(this.Factory.WallSlide());
        }
    }

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsJumpingHash, true);
        this.HandleJump();
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();

        this.Ctx.Animator.SetBool(AnimatorHelper.IsJumpingHash, false);
        this.Ctx.CurrentJumpCountResetRoutine = this.Ctx.StartCoroutine(this.JumpCountResetRoutine());
        if (this.Ctx.JumpCount == this.Ctx.MaxJumpCount)
        {
            this.Ctx.JumpCount = 0;
            this.Ctx.Animator.SetInteger(AnimatorHelper.JumpCountHash, 0);
        }
    }

    public override void InitializeSubState()
    {
        this.SetSubState(this.Factory.MidAir());
    }

    public override void UpdateState()
    {
        this.HandleGravity();

        base.UpdateState();
    }

    void HandleJump()
    {
        if (this.Ctx.JumpCount < this.Ctx.MaxJumpCount && this.Ctx.CurrentJumpCountResetRoutine != null)
        {
            this.Ctx.StopCoroutine(this.Ctx.CurrentJumpCountResetRoutine);
        }

        this.Ctx.JumpCount += 1;
        this.Ctx.Animator.SetInteger(AnimatorHelper.JumpCountHash, this.Ctx.JumpCount);
        _currentVelocityY = this.Ctx.InitialjumpVelocities[this.Ctx.JumpCount - 1];
        this.Ctx.AppliedMovementY = this.Ctx.InitialjumpVelocities[this.Ctx.JumpCount - 1];
    }

    void HandleGravity()
    {
        float fallMutiplier = this.Ctx.AppliedMovementY <= 0.0f || !this.Ctx.IsJumpPressed ? 2.0f : 1.0f;
        float previousYVelocity = _currentVelocityY;
        _currentVelocityY += (this.Ctx.JumpGravities[this.Ctx.JumpCount] * fallMutiplier * Time.deltaTime);
        this.Ctx.AppliedMovementY = (previousYVelocity + _currentVelocityY) * .5f;
    }

    IEnumerator JumpCountResetRoutine()
    {
        yield return new WaitForSeconds(.25f);
        this.Ctx.JumpCount = 0;
    }

    public bool CheckWallSlideCondition()
    {
        Ray rayFoward = new Ray(this.Ctx.CharacterController.transform.position, this.Ctx.CharacterController.transform.forward);
        Physics.Raycast(rayFoward, out RaycastHit hitDownInfo);

        return hitDownInfo.distance < 0.5f && Mathf.Abs(Vector3.Angle(Vector3.up, hitDownInfo.normal) - 90) < 10;
    }
}
