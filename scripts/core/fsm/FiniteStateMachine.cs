using System.Collections.Generic;
using Godot;
using System;

public class FiniteStateMachine
{
    protected Dictionary<Type, State> states = [];
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

    public void Add<T>(T state) where T : State
    {
        states[typeof(T)] = state;
        state.fsm = this;
    }

    public void InitState<T>() where T : State
    {
        if (!ValidateState<T>())
            return;
        CurrentState = states[typeof(T)];
        CurrentState.Enter();
    }

    public void ChangeState<T>(State previous = null) where T : State
    {
        if (!ValidateState<T>()) return;
        if (Debug) GD.Print($"{_owner} - Changing state from {nameof(PreviousState)} to {typeof(T)}");
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = states[typeof(T)];
        CurrentState.Enter(previous);
    }

    public T Get<T>() where T : State => (T)states[typeof(T)];

    public void StatePhysicsProcess(float delta)
    {
        if (!_isEnabled) return;
        CurrentState.PhysicsProcess(delta);
    }
    public void StateProcess(float delta)
    {
        if (!_isEnabled) return;
        CurrentState.Process(delta);
    }
    public void StateInput(InputEvent @event)
    {
        if (!_isEnabled) return;
        CurrentState.HandleInput(@event);
    }
    public void StateUnhandledInput(InputEvent @event)
    {
        if (!_isEnabled) return;
        CurrentState.HandleUnhandledInput(@event);
    }

    private bool ValidateState<T>() where T : State
    {
        if (!states.TryGetValue(typeof(T), out _))
        {
            GD.PushError($"{_owner}: no state '{typeof(T)}' (have: {string.Join(", ", states.Keys)})");
            return false;
        }
        return true;
    }
}