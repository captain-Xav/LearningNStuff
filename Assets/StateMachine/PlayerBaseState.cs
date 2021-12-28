using UnityEngine;

public abstract class PlayerBaseState
{
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;
    private bool _isRootState;

    protected PlayerStateMachine Ctx => _ctx;
    protected PlayerStateFactory Factory => _factory;

    protected PlayerBaseState(PlayerStateMachine ctx, PlayerStateFactory factory, bool isRootState = false)
    {
        _ctx = ctx;
        _factory = factory;
        _isRootState = isRootState;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();



    public void UpdateStates()
    {
        if (_currentSubState != null)
        {
            _currentSubState.UpdateState();
        }

        this.UpdateState();
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        // Current state exits state
        this.ExitState();
        // New state enter state
        newState.EnterState();

        if (_isRootState)
        {
            // Switch current state of context
            this.Ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            // Set the current super states sub state to the new state
            _currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
