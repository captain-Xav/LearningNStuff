using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerBaseState : BaseState<PlayerContext, PlayerStateFactory>
{
    protected PlayerBaseState(PlayerContext ctx, PlayerStateFactory factory)
        : base(ctx, factory) { }
}
