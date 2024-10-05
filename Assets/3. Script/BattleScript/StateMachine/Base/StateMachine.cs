using UnityEngine;

public class StateMachine
{
    public IState current;
    public IState prev;

    public void Init(IState state)
    {
        prev = current;
        current = state;
        state.OnEnter();
    }

    public void TransitionTo(IState next)
    {
        if (current == next) return;

        prev = current;
        prev.OnExit();
        current = next;
        next.OnEnter();
    }

    public void Update()
    {
        if (current != null)
        {
            current.OnUpdate();
        }
    }
}
