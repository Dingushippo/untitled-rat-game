using System.Collections.Generic;
using Godot;

public class FiniteStateMachine
{
    protected Dictionary<string, State> states = [];
    public State CurrentState { get; private set; }
    public string CurrentStateName { get; private set; }
    public string PreviousStateName { get; set; }

    public void Add(string key, State state)
    {
        states[key] = state;
        state.fsm = this;
    }

    public void StatePhysicsProcess(float delta) => CurrentState.PhysicsProcess(delta);

    public void StateProcess(float delta) => CurrentState.Process(delta);

    public void InitState(string newState)
    {
        CurrentState = states[newState];
        CurrentStateName = newState;
        CurrentState.Enter();
    }

    public void ChangeState(string newState, State previous = null)
    {
        CurrentState.Exit();
        CurrentState = states[newState];
        CurrentStateName = newState;
        CurrentState.Enter(previous);
    }
}