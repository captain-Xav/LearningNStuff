using UnityEngine;

public class WallSlideState : SuperState<PlayerContext, PlayerStateFactory>
{
    private bool _isWallSliding;
    private bool _isGrounded;

    public WallSlideState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    public override void CheckSwitchStates()
    {
        if (_isGrounded)
        {
            this.SwitchState(this.Factory.Grounded());
        }
        else if (!_isWallSliding)
        {
            this.SwitchState(this.Factory.Fall());
        }
    }

    public override void EnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWallSlidingHash, true);
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWallSlidingHash, false);
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (this.Ctx.CharacterController.isGrounded)
        {
            Ray rayFoward = new Ray(this.Ctx.CharacterController.transform.position, Vector3.down);
            Physics.Raycast(rayFoward, out RaycastHit hitDownInfo);

            float angle = Vector3.Angle(Vector3.up, hitDownInfo.normal) - 90;
            Debug.Log($"Angle {angle}");
            Debug.Log($"hitDownInfo.distance {hitDownInfo.distance}");
            _isGrounded = hitDownInfo.distance < 0.5f && Mathf.Abs(angle) < 5;
            Debug.Log($"_isGrounded {_isGrounded}");
        }

        if (!_isGrounded)
        {
            Ray rayFoward = new Ray(this.Ctx.CharacterController.transform.position, this.Ctx.CharacterController.transform.forward);
            Physics.Raycast(rayFoward, out RaycastHit hitFowardInfo);

            float angle = Vector3.Angle(Vector3.up, hitFowardInfo.normal) - 90;
            Debug.Log($"Angle {angle}");
            Debug.Log($"hitDownInfo.distance {hitFowardInfo.distance}");
            _isWallSliding = hitFowardInfo.distance < 0.5f && Mathf.Abs(angle) < 5;
            Debug.Log($"_isWallSliding {_isWallSliding}");

            this.Ctx.AppliedMovementY = this.Ctx.FallingGravity * 0.5f * (1 - angle / 100f);
            if (this.Ctx.CharacterController.isGrounded)
            {
                this.Ctx.AppliedMovementXZ = hitFowardInfo.normal * 0.1f;
            }
            else
            {
                this.Ctx.AppliedMovementXZ = Vector3.zero;
            }
            Debug.Log($"this.Ctx.AppliedMovementY {this.Ctx.AppliedMovementY}");
        }

        this.CheckSwitchStates();
    }

    public static bool CheckWallSlideCondition(Transform character)
    {
        Ray rayFoward = new Ray(character.position, character.forward);
        Physics.Raycast(rayFoward, out RaycastHit hitDownInfo);

        return hitDownInfo.distance < 0.5f && Mathf.Abs(Vector3.Angle(Vector3.up, hitDownInfo.normal) - 90) < 10;
    }
}
