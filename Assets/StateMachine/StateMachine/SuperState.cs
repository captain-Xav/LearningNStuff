using UnityEngine;

public abstract class SuperState<T_Context, T_Factory> : BaseState<T_Context, T_Factory>
    where T_Context : Context
    where T_Factory : StateFactory<T_Context>
{
    protected SuperState(T_Context ctx, T_Factory factory)
        : base(ctx, factory) 
    {
        this.InitializeSubState();
    }
    public override void EnterState()
    {
        this.CurrentSubState?.EnterState();
    }

    public override void ExitState()
    {
        this.CurrentSubState?.ExitState();
    }

    public virtual void InitializeSubState() { }
}
