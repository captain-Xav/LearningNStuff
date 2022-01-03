public interface IState
{
    public void CheckSwitchState();
    public void EnterState();
    public void ExitState();
    public void UpdateState();

    (string subState, string superState) GetStateTextValues();
}

