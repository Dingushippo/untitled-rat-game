using Godot;
using System;
using System.ComponentModel;

public partial class Rat : CharacterBody3D, IGrabbable, IThrowable
{
    [Export] public PathfindingComponent PathfindingComponent;
    [Export] public InteractComponent InteractComponent;
    [Export] public CollisionShape3D Collider;
    [Export] public float Speed = 10f;
    [Export] public float Acceleration = 10f;
    [Export] public Vector3 GrabOrientation { get; set; }
    [Export] public Vector3 GrabOffset { get; set; }
    [Export] bool debug = true;

    public Vector3 NavigationTargetPosition;
    private Node3D _navigationTarget;
    private Node3D _navigationTargetOriginal;
    private Player _player;

    private FiniteStateMachine _fsm;

    public override void _Ready()
    {
        if (PathfindingComponent == null)
        {
            GD.PrintErr("Rat requires a PathfindingComponent to function.");
        }


        EventBus.Subscribe(Event.DebugAimMarker, OnDebugMouseClick);

        _player = GetTree().GetFirstNodeInGroup("player") as Player;
        InitStateMachine();
        // InteractComponent.OnInteract += OnInteract;
        // CallDeferred(nameof(InitializeNavigationTarget));
    }

    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);

    private void InitStateMachine()
    {
        _fsm = new FiniteStateMachine();
        _fsm.Add("follow", new RatFollowState(this, PathfindingComponent));
        _fsm.Add("target_reached", new RatTargetReachedState(this));
        _fsm.Add("idle", new RatIdleState(this));
        _fsm.InitState("idle");
    }

    private void OnInteract()
    {
        Grab();
    }

    public void Grab()
    {
        PathfindingComponent.DisablePathfinding();
        Collider.Disabled = true;
        _player.InitiateGrabCycle(this);
    }

    public void Release()
    {
        PathfindingComponent.EnablePathfinding();
        Collider.Disabled = false;
    }

    public void Throw(Vector3 direction, float force)
    {
        PathfindingComponent.ApplyForce(direction * force);
        Collider.Disabled = false;
    }
    public void Throw(Vector3 direction, Vector3 position)
    {
        PathfindingComponent.ApplyForceTowardsPosition(position);
        Collider.Disabled = false;
    }

    private void InitializeNavigationTarget()
    {
        if (PathfindingComponent == null)
        {
            return;
        }

        if (PathfindingComponent.NavigationTarget != null)
        {
            _navigationTarget = PathfindingComponent.NavigationTarget;
            return;
        }

        Node parent = GetTree().CurrentScene;
        if (parent == null)
        {
            GD.PrintErr("Unable to add NavigationTarget because the current scene is not available yet.");
            return;
        }

        _navigationTarget = new Node3D
        {
            Name = "NavigationTarget",
        };
        _navigationTargetOriginal = _navigationTarget;

        parent.AddChild(_navigationTarget);
        _navigationTarget.GlobalPosition = GlobalPosition;
        _navigationTarget.Owner = parent;
        PathfindingComponent.NavigationTarget = _navigationTarget;

        GD.Print("NavigationTarget created and added to the scene tree.");
    }

    private void OnDebugMouseClick(object[] args)
    {
        Vector3 pos = (Vector3)args[0];

        GD.Print($"Setting TargetPosition to {pos}");
        NavigationTargetPosition = pos;
        _fsm.ChangeState("follow");
        // PathfindingComponent.NavigationTarget.GlobalPosition = pos;

    }
}
