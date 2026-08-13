using System.Collections.Generic;
using Godot;
using System;

public class FiniteStateMachine
{
    protected Dictionary<string, State> states = [];
    protected string _owner = null;
    public State CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }
    public string PreviousStateName { get; set; }
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
        states[nameof(T)] = state;
        state.fsm = this;
    }

    public void InitState<T>() where T : State
    {
        string state = nameof(T);
        if (!ValidateState(state))
            return;
        CurrentState = states[state];
        CurrentStateName = state;
        CurrentState.Enter();
    }

    public void ChangeState<T>(State previous = null) where T : State
    {
        string state = nameof(T);
        if (!ValidateState(state)) return;
        if (Debug) GD.Print($"{_owner} - Changing state from {CurrentStateName} to {state}");
        PreviousStateName = CurrentStateName;
        CurrentState.Exit();
        CurrentState = states[state];
        CurrentStateName = state;
        CurrentState.Enter(previous);
    }

    public T Get<T>() where T : State => (T)states[nameof(T)];

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

    private bool ValidateState(string state)
    {
        if (!states.TryGetValue(state, out State next))
        {
            GD.PushError($"{_owner}: no state '{state}' (have: {string.Join(", ", states.Keys)})");
            return false;
        }
        return true;
    }
}