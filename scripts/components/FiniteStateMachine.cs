using System.Collections.Generic;
using Godot;

public class FiniteStateMachine
{
    protected Dictionary<string, State> states = [];
    public State CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }
    public string PreviousStateName { get; set; }

    public bool Debug { get; set; } = false;

    public void Add(string key, State state)
    {
        states[key] = state;
        state.fsm = this;
    }

    public void StatePhysicsProcess(float delta) => CurrentState.PhysicsProcess(delta);
    public void StateProcess(float delta) => CurrentState.Process(delta);
    public void StateInput(InputEvent @event) => CurrentState.HandleInput(@event);
    public void StateUnhandledInput(InputEvent @event) => CurrentState.HandleUnhandledInput(@event);

    public void InitState(string newState)
    {
        CurrentState = states[newState];
        CurrentStateName = newState;
        CurrentState.Enter();
    }

    public void ChangeState(string newState, State previous = null)
    {
        if (Debug) GD.Print($"Changing state from {CurrentStateName} to {newState}");

        CurrentState.Exit();
        CurrentState = states[newState];
        CurrentStateName = newState;
        CurrentState.Enter(previous);
    }
}