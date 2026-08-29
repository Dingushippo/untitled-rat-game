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
        GD.Print($"Testing: {GetChildren()}");
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
                GD.Print($"Addong state: {state.Name}, parent: {parent?.Name}");

            _states.Add(state.GetType(), state);
            state.Init(owner, this, parent);
            RegisterStatesRecursive(child, owner, state);
        }
    }

    public void ChangeState<T>()
        where T : State
    {
        Type stateKey = typeof(T);
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
            GD.Print($"{_owner?.Name} changed state from {_prevState?.Name} to {_currentState?.Name}");
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
