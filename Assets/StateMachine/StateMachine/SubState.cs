using UnityEngine;

public abstract class SubState<T_Context, T_Factory> : BaseState<T_Context, T_Factory>
    where T_Context : Context
    where T_Factory : StateFactory<T_Context>
{
    protected SubState(T_Context ctx, T_Factory factory)
        : base(ctx, factory) { }
}
