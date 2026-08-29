using System;
using System.Collections.Generic;
using Godot;


[GlobalClass]
public partial class HierarchicalStateMachine : Node
{
    [Export]
    public NodePath InitialStatePath;

    [Export]
    public bool Debug;
    private Dictionary<Type, State> _states = [];
    private Node _owner;

    private State _currentState;
    private State _prevState;

    public void Init(Node owner)
    {
        _owner = owner;

        RegisterStatesRecursive(this, owner, null);

        _currentState = GetNode<State>(InitialStatePath);
        _currentState.Enter();
    }

    public void RegisterStatesRecursive(Node currentNode, Node owner, State parent)
    {
        foreach (Node child in currentNode.GetChildren())
        {
            if (child is not State state)
                return;
            if (Debug)
                GD.Print($"Adding state: {state.Name}, parent: {parent?.Name}");

            _states.Add(state.GetType(), state);
            state.Init(owner, this, parent);
            RegisterStatesRecursive(child, owner, state);
        }
    }

    public void ChangeState<T>()
        where T : State
    {
        if (IsState<T>())
            return;
        Type stateKey = typeof(T);
        if (!_states.ContainsKey(stateKey))
        {
            GD.PushError($"{_owner} hfsm does not contain state: {stateKey}");
            return;
        }
        State newState = _states[stateKey];

        RecursiveExit(_currentState);
        _prevState = _currentState;
        RecursiveEnter(newState);
        _currentState = newState;

        if (Debug)
            GD.Print($"{_owner.Name} changed state from {_prevState?.Name} to {_currentState?.Name}");
    }

    private void RecursiveExit(State state, int depth = 0)
    {
        if (depth >= 10 || state.Parent == null)
            return;

        if (_currentState == state.Parent)
            return;

        state.Exit();
        RecursiveExit(state.Parent, depth + 1);
    }

    private void RecursiveEnter(State state, int depth = 0)
    {
        if (depth >= 10 || state == _currentState)
            return;

        state.Enter();
        if (state.Parent != null)
            RecursiveEnter(state.Parent, depth + 1);
    }

    public bool IsStateBranch<T>()
        where T : State
    {
        if (IsState<T>())
            return true;

        State checkState = _currentState;
        State state = _states[typeof(T)];

        for (State check = _currentState; check.Parent != null; check = check.Parent)
        {
            if (checkState.Parent == state)
                return true;
        }
        return false;
    }
    public bool IsState<T>()
        where T : State
    {
        return _currentState.GetType() == typeof(T);
    }

    public override void _PhysicsProcess(double delta) =>
        _currentState.PhysicsProcess((float)delta);

    public override void _Process(double delta) => _currentState.Process((float)delta);
}
