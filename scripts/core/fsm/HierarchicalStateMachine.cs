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
        State lca = FindLowestCommonAncestor(_currentState, newState);

        ExitToAncestor(_currentState, lca);
        _prevState = _currentState;
        _currentState = newState;
        EnterFromAncestor(newState, lca);

        if (Debug)
            GD.Print($"{_owner.Name} changed state from {_prevState.Name} to {_currentState.Name}");
    }

    private State FindLowestCommonAncestor(State a, State b)
    {
        HashSet<State> path = new();
        for (State current = a; current != null; current = current.Parent)
            path.Add(current);

        for (State current = b; current != null; current = current.Parent)
        {
            if (path.Contains(current))
                return current;
        }
        return null;
    }

    private void ExitToAncestor(State current, State ancestor)
    {
        for (State state = current; state != null && state != ancestor; state = state.Parent)
            state.Exit();
    }

    private void EnterFromAncestor(State target, State ancestor)
    {
        List<State> path = new();
        for (State state = target; state != null && state != ancestor; state = state.Parent)
            path.Add(state);

        path.Reverse();
        foreach (State state in path)
            state.Enter();
    }

    public bool IsStateBranch<T>()
        where T : State
    {
        State state = _states[typeof(T)];
        return IsStateBranch(state);
    }

    public bool IsStateBranch(State state)
    {
        if (IsState(state))
            return true;

        State checkState = _currentState;

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

    public bool IsState(State state)
    {
        return _currentState == state;
    }

    private bool IsInActiveBranch(State state)
    {
        for (State current = _currentState; current != null; current = current.Parent)
            if (current == state) return true;
        return false;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        List<State> activeBranch = new();

        for (State current = _currentState; current != null; current = current.Parent)
            activeBranch.Add(current);

        // Process top-down
        activeBranch.Reverse();
        foreach (State state in activeBranch)
        {
            if (!IsInActiveBranch(state))
                break;
            state.PhysicsProcess(dt);
        }

    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        List<State> activeBranch = new();

        for (State current = _currentState; current != null; current = current.Parent)
            activeBranch.Add(current);

        // Process top-down
        activeBranch.Reverse();
        foreach (State state in activeBranch)
        {
            if (!IsInActiveBranch(state))
                break;
            state.Process(dt);
        }
    }
}
