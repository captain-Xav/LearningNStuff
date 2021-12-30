using System.Collections.Generic;

public enum PlayerState
{
    Grounded,
    Jump,
    Fall,
    WallSlide,
    Idle,
    Walk,
    Run,
    MidAir
}

public class PlayerStateFactory : StateFactory<PlayerContext>
{
    private Dictionary<PlayerState, BaseState<PlayerContext, PlayerStateFactory>> State = new Dictionary<PlayerState, BaseState<PlayerContext, PlayerStateFactory>>();

    public PlayerStateFactory(PlayerContext currentContext)
        : base(currentContext) { }

    public BaseState<PlayerContext, PlayerStateFactory> GetState(PlayerState stateToGet)
    {
        if (!this.State.TryGetValue(stateToGet, out BaseState<PlayerContext, PlayerStateFactory> state))
        {
            state = this.CreateState(stateToGet);
            this.State.Add(stateToGet, state);
        }

        return state;
    }

    private BaseState<PlayerContext, PlayerStateFactory> CreateState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Grounded:
                return new PlayerGroundedState(this.Context, this);
            case PlayerState.Jump:
                return new PlayerJumpState(this.Context, this);
            case PlayerState.Fall:
                return new PlayerFallState(this.Context, this);
            case PlayerState.WallSlide:
                return new PlayerWallSlideState(this.Context, this);
            case PlayerState.Idle:
                return new PlayerIdleState(this.Context, this);
            case PlayerState.Walk:
                return new PlayerWalkState(this.Context, this);
            case PlayerState.Run:
                return new PlayerRunState(this.Context, this);
            case PlayerState.MidAir:
                return new PlayerMidAirState(this.Context, this);
            default:
                return new PlayerGroundedState(this.Context, this);

        }
    }
}
