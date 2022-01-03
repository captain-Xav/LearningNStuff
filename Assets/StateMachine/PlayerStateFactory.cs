using System;
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
    private Dictionary<PlayerState, BaseState<PlayerContext, PlayerStateFactory>> _state = new Dictionary<PlayerState, BaseState<PlayerContext, PlayerStateFactory>>();

    public PlayerStateFactory(PlayerContext currentContext)
        : base(currentContext)
    {
        _state = new Dictionary<PlayerState, BaseState<PlayerContext, PlayerStateFactory>>()
        {
            { PlayerState.Grounded, new PlayerGroundedState(this.Context, this) },
            { PlayerState.Jump, new PlayerJumpState(this.Context, this) },
            { PlayerState.Fall, new PlayerFallState(this.Context, this) },
            { PlayerState.WallSlide, new PlayerWallSlideState(this.Context, this) },
            { PlayerState.Idle, new PlayerIdleState(this.Context, this) },
            { PlayerState.Walk, new PlayerWalkState(this.Context, this) },
            { PlayerState.Run, new PlayerRunState(this.Context, this) },
            { PlayerState.MidAir, new PlayerMidAirState(this.Context, this) }
        };
    }

    public BaseState<PlayerContext, PlayerStateFactory> GetState(PlayerState stateToGet)
    {
        if (!this._state.TryGetValue(stateToGet, out BaseState<PlayerContext, PlayerStateFactory> state))
        {
            throw new Exception("Each state must be initialize in the constructor beforehand");
        }

        return state;
    }
}
