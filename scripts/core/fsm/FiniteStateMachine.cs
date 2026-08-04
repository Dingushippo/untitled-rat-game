using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Godot;

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

    public void Add(string key, State state)
    {
        states[key] = state;
        state.fsm = this;
    }

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

    public void InitState(string newState)
    {
        if (!ValidateState(newState)) return;
        CurrentState = states[newState];
        CurrentStateName = newState;
        CurrentState.Enter();
    }

    public void ChangeState(string newState, State previous = null)
    {
        if (!ValidateState(newState)) return;
        if (Debug) GD.Print($"{_owner} - Changing state from {CurrentStateName} to {newState}");
        PreviousStateName = CurrentStateName;
        CurrentState.Exit();
        CurrentState = states[newState];
        CurrentStateName = newState;
        CurrentState.Enter(previous);
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