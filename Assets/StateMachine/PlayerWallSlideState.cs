using UnityEngine;

public class PlayerWallSlideState : BaseState<PlayerContext, PlayerStateFactory>
{
    private bool _isWallSliding;
    private bool _isGrounded;

    public PlayerWallSlideState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    protected override void OnCheckSwitchState()
    {
        if (_isGrounded)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Grounded));
        }
        else if (!_isWallSliding)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Fall));
        }
    }

    protected override void OnEnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWallSlidingHash, true);
    }

    protected override void OnExitState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWallSlidingHash, false);
    }

    protected override void OnUpdateState()
    {
        if (this.Ctx.CharacterPhysics.IsGrounded)
        {
            float angle = Vector3.Angle(Vector3.up, this.Ctx.CharacterPhysics.GroundHitInfo.normal) - 90;
            Debug.Log($"Angle1 {angle}");
            _isGrounded = Mathf.Abs(angle) > 5;
            Debug.Log($"_isGrounded {_isGrounded}");
        }

        if (!_isGrounded)
        {
            _isWallSliding = PlayerWallSlideState.IsWallSliding(this.Ctx.CharacterController, out RaycastHit hitFowardInfo);

            if (_isWallSliding)
            {

                Vector3 pente = Vector3.Cross(hitFowardInfo.normal, Vector3.right);
                Vector3 projection = Vector3.Project(Vector3.up * this.Ctx.FallingGravity * 0.5f, pente.normalized);


                this.Ctx.AppliedMovementY = projection.y;
                this.Ctx.AppliedMovementXZ = projection.XZPlane(); ;
            }
        }
    }

    public static bool IsWallSliding(CharacterController character, out RaycastHit hitFowardInfo)
    {
        Physics.Raycast(character.transform.position + character.center, character.transform.forward, out hitFowardInfo, 0.5f);
        float angle = Vector3.Angle(Vector3.up, hitFowardInfo.normal) - 90;
        return Mathf.Abs(angle) < 5;
    }
}
