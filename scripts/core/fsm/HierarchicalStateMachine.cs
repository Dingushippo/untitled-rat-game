using System;
using System.Collections.Generic;
using Godot;

public partial class HierarchicalStateMachine<T> : Node
    where T : TypedState<T>
{
    [Export]
    public NodePath InitialStatePath;

    [Export]
    public bool Debug;
    private Dictionary<Type, T> _states = [];
    private Node _owner;

    private T _currentState;
    private T _prevState;

    public void Init(Node owner)
    {
        RegisterStatesRecursive(this, owner, null);

        _currentState = GetNode<T>(InitialStatePath);
        _currentState.Enter();
    }

    public void RegisterStatesRecursive(Node currentNode, Node owner, State parent)
    {
        foreach (Node child in currentNode.GetChildren())
        {
            if (child is not T state)
                return;

            _states.Add(state.GetType(), state);
            state.Init(owner, this, parent);
            RegisterStatesRecursive(child, owner, state);
        }
    }

    public void ChangeState<ST>()
        where ST : T
    {
        Type stateKey = typeof(ST);
        if (!_states.ContainsKey(stateKey))
        {
            GD.PushError($"{_owner} hfsm does not contain state: {stateKey}");
            return;
        }

        _currentState.Exit();
        _prevState = _currentState;
        _currentState = _states[stateKey];
        _currentState.Enter();

        if (Debug)
            GD.Print($"{_owner} changed state from {_prevState} to {_currentState}");
    }

    public bool IsState<ST>()
        where ST : T
    {
        return _currentState.GetType() == typeof(ST);
    }

    public override void _PhysicsProcess(double delta) =>
        _currentState.PhysicsProcess((float)delta);

    public override void _Process(double delta) => _currentState.Process((float)delta);
}
