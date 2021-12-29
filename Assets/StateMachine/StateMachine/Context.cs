using UnityEngine;

public abstract class Context : MonoBehaviour
{
    public IState CurrentState { get; set; }
}
