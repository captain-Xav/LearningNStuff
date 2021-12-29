using UnityEngine;

public abstract class BaseState<T_Context, T_Factory> : IState
    where T_Context : Context
    where T_Factory : StateFactory<T_Context>
{
    protected T_Context Ctx { get; private set; }
    protected T_Factory Factory { get; private set; }
    public BaseState<T_Context, T_Factory> CurrentSubState { get; private set; }
    public BaseState<T_Context, T_Factory> CurrentSuperState { get; private set; }

    protected BaseState(T_Context ctx, T_Factory factory)
    {
        this.Ctx = ctx;
        this.Factory = factory;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void CheckSwitchStates() { }
    public virtual void UpdateState()
    {
        this.CheckSwitchStates();
    }

    public void UpdateStates()
    {
        if (this.CurrentSubState != null)
        {
            this.CurrentSubState.UpdateState();
        }

        this.UpdateState();
    }

    protected void SwitchState(BaseState<T_Context, T_Factory> newState)
    {
        // Current state exits state
        this.ExitState();
        // New state enter state
        newState.EnterState();

        if (newState is SuperState<T_Context, T_Factory>)
        {
            // Switch current state of context
            this.Ctx.CurrentState = newState;
        }
        else if (this.CurrentSuperState != null)
        {
            // Set the current super states sub state to the new state
            this.CurrentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(BaseState<T_Context, T_Factory> newSuperState)
    {
        this.CurrentSuperState = newSuperState;
    }

    protected void SetSubState(BaseState<T_Context, T_Factory> newSubState)
    {
        this.CurrentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

    public (string subState, string superState) GetStateTextValues()
    {
        if (this is SuperState<T_Context, T_Factory>)
        {
            return (this.CurrentSubState?.ToString(), this.ToString());
        }
        else
        {
            return (this.ToString(), this.CurrentSuperState?.ToString());
        }
    }
}
