public class PlayerStateFactory : StateFactory<PlayerContext>
{

    public PlayerStateFactory(PlayerContext currentContext)
        : base(currentContext) { }

    public BaseState<PlayerContext, PlayerStateFactory> Idle()
    {
        return new PlayerIddleState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> Walk()
    {
        return new PlayerWalkState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> Run()
    {
        return new PlayerRunState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> Jump()
    {
        return new PlayerJumpState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> Grounded()
    {
        return new PlayerGroundedState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> Fall()
    {
        return new PlayerFallState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> MidAir()
    {
        return new MidAirState(this.Context, this);
    }

    public BaseState<PlayerContext, PlayerStateFactory> WallSlide()
    {
        return new WallSlideState(this.Context, this);
    }
}
