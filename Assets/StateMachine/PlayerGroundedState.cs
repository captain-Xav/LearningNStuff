using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : SuperState<PlayerContext, PlayerStateFactory>
{
    bool _isFalling = false;
    float _jumpBuffer = 0f;

    public PlayerGroundedState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

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
        this.Ctx.Animator.SetBool(AnimatorHelper.IsGroundedHash, true);
        this.Ctx.AppliedMovementY = this.Ctx.GroundedGravity;
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();

        this.Ctx.Animator.SetBool(AnimatorHelper.IsGroundedHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWalkingHash, false);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsRunningHash, false);
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
        this.Ctx.AppliedMovementY = this.Ctx.GroundedGravity;

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
            // Debug.DrawRay(position, rayDown.direction, Color.blue, 3f);

            ////if (hitDownInfo.distance > 0.25f)
            ////    Debug.Log($"hitDownInfo.distance {hitDownInfo.distance}");

            if (hitDownInfo.distance < 0.3f)
            {
                // position.y = hitDownInfo.point.y + (0.5f * Vector3.Angle(Vector3.up, hitDownInfo.normal) / 100f) ;
                Vector3 pente = Vector3.Cross(Vector3.Cross(Vector3.up, this.Ctx.AppliedMovementXZ), hitDownInfo.normal);
                // Debug.DrawRay(position, pente, Color.red, 3f);
                //Debug.DrawLine(position, position + this.Ctx.AppliedMovementXZ * Time.deltaTime, Color.green, 3f);
                Debug.DrawRay(position, this.Ctx.AppliedMovementXZ, Color.green, 3f);
                Debug.DrawRay(position, pente, Color.red, 3f);

                float angle = Vector3.Angle(this.Ctx.AppliedMovementXZ, pente);
                Debug.Log($"pente: {this.Ctx.CharacterController.transform.forward}");
                Debug.Log($"AppliedMovementXZ: {this.Ctx.AppliedMovementXZ.normalized}");
                Debug.Log($"pente: {pente.normalized}");
                Debug.Log($"angle: {angle}");
                Debug.Log($"sdj: {this.Ctx.AppliedMovementXZ.magnitude}");
                Debug.Log($"opp: {Mathf.Tan(Mathf.Deg2Rad * angle) * this.Ctx.AppliedMovementXZ.magnitude}");
                //Debug.DrawLine(position, position + projection, Color.blue, 3f);
                Debug.DrawRay(position, new Vector3(this.Ctx.AppliedMovementXZ.x, Mathf.Min(-Mathf.Tan(Mathf.Deg2Rad * angle) * this.Ctx.AppliedMovementXZ.magnitude, this.Ctx.GroundedGravity), this.Ctx.AppliedMovementXZ.z), Color.blue, 3f);

                // this.Ctx.CharacterController.enabled = false;
                // this.Ctx.CharacterController.transform.position = position;
                // this.Ctx.CharacterController.enabled = true;
                this.Ctx.AppliedMovementY = Mathf.Min(-Mathf.Tan(Mathf.Deg2Rad * angle) * this.Ctx.AppliedMovementXZ.magnitude, this.Ctx.GroundedGravity);
                Debug.Log($"Y: {this.Ctx.AppliedMovementY}");

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

        base.UpdateState();
    }
}
