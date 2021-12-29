public interface IState
{
    void EnterState();
    void ExitState();

    void UpdateStates();

    (string subState, string superState) GetStateTextValues();
}

