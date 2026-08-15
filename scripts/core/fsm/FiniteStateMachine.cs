using Godot;
using System;
using System.Collections.Generic;

public class FiniteStateMachine<T>
    where T : State<T>
{
    protected Dictionary<Type, T> _states = [];
    protected string _owner = null;
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }
    public bool Debug { get; set; } = false;

    private bool _isEnabled = true;

    public FiniteStateMachine(Node owner)
    {
        _owner = owner.Name;
    }

    public void SetEnabled(bool enabled)
    {
        _isEnabled = enabled;
    }

    public void Add<ST>(ST state)
        where ST : T
    {
        _states[typeof(ST)] = state;
        state.fsm = this;
    }

    public void InitState<ST>()
        where ST : T
    {
        if (!ValidateState<ST>())
            return;
        CurrentState = _states[typeof(ST)];
        CurrentState.Enter();
    }

    public void ChangeState<ST>(State previous = null)
        where ST : T
    {
        if (!ValidateState<ST>())
            return;
        if (Debug)
            GD.Print($"{_owner} - Changing state from {PreviousState} to {typeof(ST)}");
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = _states[typeof(ST)];
        CurrentState.Enter(previous);
    }

    public ST Get<ST>()
        where ST : T => (ST)_states[typeof(ST)];

    public void StatePhysicsProcess(float delta)
    {
        if (!_isEnabled)
            return;
        CurrentState.PhysicsProcess(delta);
    }

    public void StateProcess(float delta)
    {
        if (!_isEnabled)
            return;
        CurrentState.Process(delta);
    }

    public void StateInput(InputEvent @event)
    {
        if (!_isEnabled)
            return;
        CurrentState.HandleInput(@event);
    }

    public void StateUnhandledInput(InputEvent @event)
    {
        if (!_isEnabled)
            return;
        CurrentState.HandleUnhandledInput(@event);
    }

    private bool ValidateState<ST>()
        where ST : T
    {
        if (!_states.TryGetValue(typeof(ST), out _))
        {
            GD.PushError(
                $"{_owner}: no state '{typeof(ST)}' (have: {string.Join(", ", _states.Keys)})"
            );
            return false;
        }
        return true;
    }
}