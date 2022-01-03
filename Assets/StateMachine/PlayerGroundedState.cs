using UnityEngine;

public class PlayerGroundedState : BaseState<PlayerContext, PlayerStateFactory>
{
    bool _isFalling = false;
    float _jumpBuffer = 0f;

    public PlayerGroundedState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    protected override void OnCheckSwitchState()
    {
        // If player is grounded and jump is pressed, switch to jump state
        if (this.Ctx.IsJumpPressed && Time.time - 0.1f < this.Ctx.JumpPressTimer)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Jump));
        }
        else if (_isFalling && _jumpBuffer > 0.1f)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Fall));
        }
    }

    protected override void OnEnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsGroundedHash, true);
        this.Ctx.AppliedMovementY = this.Ctx.GroundedGravity;
    }

    protected override void OnExitState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsGroundedHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWalkingHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsRunningHash, false);
    }

    protected override void OnInitializeSubState()
    {
        if (!this.Ctx.IsMovementPressed)
        {
            this.SetSubState(this.Factory.GetState(PlayerState.Idle));
        }
        else if (this.Ctx.IsMovementPressed && !this.Ctx.IsRunPressed)
        {
            this.SetSubState(this.Factory.GetState(PlayerState.Walk));
        }
        else
        {
            this.SetSubState(this.Factory.GetState(PlayerState.Run));
        }
    }

    protected override void OnUpdateState()
    {
        this.Ctx.AppliedMovementY = this.Ctx.GroundedGravity;

        if (this.Ctx.CharacterPhysics.IsGrounded)
        {
            _jumpBuffer = 0f;
            _isFalling = false;

            if (this.Ctx.CharacterPhysics.GroundHitInfo.normal != Vector3.up)
            {
                Vector3 pente = Vector3.Cross(Vector3.Cross(Vector3.up, this.Ctx.AppliedMovementXZ), this.Ctx.CharacterPhysics.GroundHitInfo.normal);
                Vector3 projection = Vector3.Project(this.Ctx.AppliedMovementXZ, pente.normalized);

                Debug.DrawRay(this.Ctx.CharacterController.transform.position, this.Ctx.AppliedMovementXZ, Color.green, 3f);
                Debug.DrawRay(this.Ctx.CharacterController.transform.position, pente, Color.red, 3f);
                Debug.DrawRay(this.Ctx.CharacterController.transform.position, projection, Color.blue, 3f);

                this.Ctx.AppliedMovementY = projection.y;
                this.Ctx.AppliedMovementXZ = projection.XZPlane();
            }
        }
        else
        {
            _isFalling = true;
            _jumpBuffer += Time.deltaTime;
            this.Ctx.AppliedMovementY = this.Ctx.FallingGravity;
        }
    }
}
