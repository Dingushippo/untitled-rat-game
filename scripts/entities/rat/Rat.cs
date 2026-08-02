using Godot;
using System;
using System.ComponentModel;

public partial class Rat : CharacterBody3D
{
    [Export] public NavigationAgent3D NavAgent;
    [Export] public CollisionShape3D Collider;
    [Export] public InteractAreaComponent InteractArea;
    [Export] public RatDef RatDef;
    [Export] public RatFlightTuning FlightTuning;
    [Export] public float Speed = 10f;
    [Export] public float Acceleration = 10f;
    [Export] public Vector3 GrabOrientation { get; set; }
    [Export] public Vector3 GrabOffset { get; set; }
    [Export]
    public bool Debug
    {
        get => _debug;
        set
        {
            _debug = value;
            _fsm.Debug = _debug;
        }
    }
    private bool _debug;
    public Inventory Cargo;
    public Vector3 NavigationTargetPosition;
    private Node3D _navigationTarget;
    private Node3D _navigationTargetOriginal;
    private FiniteStateMachine _fsm;

    public override void _Ready()
    {
        FlightTuning ??= new RatFlightTuning();

        if (NavAgent == null)
        {
            GD.PrintErr("Rat requires a NavigationAgent3D to function.");
        }
        Cargo = new Inventory(RatDef.MaxCapacity);
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
        _fsm.Add("idle", new RatIdleState(this));
        _fsm.Add("falling", new RatFallingState(this));
        _fsm.Add("landed", new RatLandedState(this));
        _fsm.Add("slotted", new RatSlottedState(this));
        _fsm.Add("intake", new RatIntakeState(this));
        _fsm.InitState("idle");
        _fsm.Debug = _debug;
    }

    public void SetNavAgentEnabled(bool enabled)
    {
        NavAgent.SetProcess(enabled);
        NavAgent.SetPhysicsProcess(enabled);
    }
}
