using UnityEngine;

public abstract class BaseState<T_Context, T_Factory> : IState
    where T_Context : Context
    where T_Factory : StateFactory<T_Context>
{
    protected T_Context Ctx { get; private set; }
    protected T_Factory Factory { get; private set; }
    private BaseState<T_Context, T_Factory> SubState { get; set; }
    private BaseState<T_Context, T_Factory> SuperState { get; set; }

    protected BaseState(T_Context ctx, T_Factory factory)
    {
        this.Ctx = ctx;
        this.Factory = factory;
    }

    protected virtual void OnCheckSwitchState() { }
    protected virtual void OnEnterState() { }
    protected virtual void OnExitState() { }
    protected virtual void OnInitializeSubState() { }
    protected virtual void OnUpdateState() { }

    public void CheckSwitchState()
    {
        this.OnCheckSwitchState();

        if (this.SubState != null)
        {
            this.SubState.OnCheckSwitchState();
        }
    }

    public void EnterState()
    {
        this.OnInitializeSubState();
        this.OnEnterState();
        this.SubState?.EnterState();
    }

    public void ExitState()
    {
        this.SubState?.EnterState();
        this.OnExitState();
    }

    public void UpdateState()
    {
        if (this.SubState != null)
        {
            this.SubState.UpdateState();
        }

        this.OnUpdateState();
    }

    protected void SwitchState(BaseState<T_Context, T_Factory> newState)
    {
        // Current state exits state
        this.ExitState();
        // New state enter state
        newState.EnterState();

        if (this.SuperState == null)
        {
            // Switch current state of context
            this.Ctx.CurrentState = newState;
        }
        else
        {
            // Set the current super states sub state to the new state
            this.SuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(BaseState<T_Context, T_Factory> newSuperState)
    {
        this.SuperState = newSuperState;
    }

    protected void SetSubState(BaseState<T_Context, T_Factory> newSubState)
    {
        this.SubState = newSubState;
        newSubState.SetSuperState(this);
    }

    public (string subState, string superState) GetStateTextValues()
    {
        if (this.SuperState == null)
        {
            return (this.SubState?.ToString(), this.ToString());
        }
        else
        {
            return (this.ToString(), this.SuperState?.ToString());
        }
    }
}
