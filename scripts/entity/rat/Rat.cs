using Godot;
using System;
using System.ComponentModel;

public partial class Rat : CharacterBody3D
{
    [Export] public NavigationAgent3D NavAgent;
    [Export] public CollisionShape3D Collider;
    [Export] public float Speed = 10f;
    [Export] public float Acceleration = 10f;
    [Export] public Vector3 GrabOrientation { get; set; }
    [Export] public Vector3 GrabOffset { get; set; }
    [Export] bool debug = true;

    public Vector3 NavigationTargetPosition;
    private Node3D _navigationTarget;
    private Node3D _navigationTargetOriginal;

    private FiniteStateMachine _fsm;

    public override void _Ready()
    {
        if (NavAgent == null)
        {
            GD.PrintErr("Rat requires a NavigationAgent3D to function.");
        }

        // EventBus.Subscribe(Event.DebugAimMarker, OnDebugMouseClick);

        InitStateMachine();
    }

    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);

    public void RevertToPrevState() => _fsm.ChangeState(_fsm.PreviousStateName);
    public void SetIdle() => _fsm.ChangeState("idle");

    public void InjectState(string key, RatState state)
    {
        _fsm.Add(key, state);
        _fsm.ChangeState(key);
    }

    private void InitStateMachine()
    {
        _fsm = new FiniteStateMachine(this);
        _fsm.Add("follow", new RatFollowState(this, NavAgent));
        _fsm.Add("target_reached", new RatTargetReachedState(this));
        _fsm.Add("idle", new RatIdleState(this));
        _fsm.InitState("idle");
        _fsm.Debug = true;
    }


    private void OnDebugMouseClick(object[] args)
    {
        Vector3 pos = (Vector3)args[0];
        GD.Print($"Setting TargetPosition to {pos}");
        NavigationTargetPosition = pos;
        _fsm.ChangeState("follow");
    }
}
