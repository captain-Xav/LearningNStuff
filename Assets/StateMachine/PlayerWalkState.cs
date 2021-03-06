using UnityEngine;

public class PlayerWalkState : BaseState<PlayerContext, PlayerStateFactory>
{
    float _factor = 1f;
    // sfloat _timer = 0f;

    public PlayerWalkState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }

    protected override void OnCheckSwitchState()
    {
        if (!this.Ctx.IsMovementPressed)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Idle));
        }
        else if (this.Ctx.IsRunPressed)
        {
            this.SwitchState(this.Factory.GetState(PlayerState.Run));
        }
    }

    protected override void OnEnterState()
    {
        this.Ctx.Animator.SetBool(AnimatorHelper.IsWalkingHash, true);
        this.Ctx.Animator.SetBool(AnimatorHelper.IsRunningHash, false);
    }

    protected override void OnUpdateState()
    {
        ////_timer += Time.deltaTime;

        ////if (_timer > 0.5f)
        ////{
        ////    Vector3 position = this.Ctx.CharacterController.transform.position;
        ////    Ray rayDown = new Ray(position, Vector3.down);
        ////    Physics.Raycast(rayDown, out RaycastHit hitDownInfo);

        ////    _factor = (180 - Vector3.Angle(this.Ctx.CharacterController.transform.forward, hitDownInfo.normal)) / 90;

        ////    _timer = 0f;
        ////}

        this.Ctx.AppliedMovementXZ = this.Ctx.CurrentMovement * this.Ctx.WalkSpeed * _factor;
    }
}
