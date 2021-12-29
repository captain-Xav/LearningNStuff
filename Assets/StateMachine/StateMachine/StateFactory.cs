using UnityEngine;

public abstract class StateFactory<T_Context>
    where T_Context : Context
{
    protected T_Context Context { get; }

    protected StateFactory(T_Context currentContext)
    {
        this.Context = currentContext;
    }
}
