using UnityEngine;

public class PlayerFallState : SuperState<PlayerContext, PlayerStateFactory>
{
    float _gravity;
    bool _isGrounded = false;

    public PlayerFallState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    public override void CheckSwitchStates()
    {
        if (_isGrounded || this.Ctx.CharacterController.isGrounded)
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
        this.Ctx.Animator.SetBool(AnimatorHelper.IsFallingHash, true);
        _gravity = this.Ctx.FallingGravity;
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();

        this.Ctx.Animator.SetBool(AnimatorHelper.IsFallingHash, false);
    }

    public override void InitializeSubState()
    {
        this.SetSubState(this.Factory.MidAir());
    }

    public override void UpdateState()
    {
        this.Ctx.AppliedMovementY = _gravity *= 1.10f;

        Ray rayDown = new Ray(this.Ctx.CharacterController.transform.position, Vector3.down);
        Physics.Raycast(rayDown, out RaycastHit hitDownInfo);

        if (hitDownInfo.distance < 0.1f)
            _isGrounded = true;


        base.UpdateState();
    }

    public bool CheckWallSlideCondition()
    {
        Ray rayFoward = new Ray(this.Ctx.CharacterController.transform.position, this.Ctx.CharacterController.transform.forward);
        Physics.Raycast(rayFoward, out RaycastHit hitDownInfo);

        return hitDownInfo.distance < 0.5f && Mathf.Abs(Vector3.Angle(Vector3.up, hitDownInfo.normal) - 90) < 10;
    }
}
