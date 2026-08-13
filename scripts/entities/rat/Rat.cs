using Godot;
using System;
using System.ComponentModel;

public partial class Rat : CharacterBody3D
{
    [Export] public NavigationAgent3D NavAgent;
    [Export] public CollisionShape3D Collider;
    [Export] public InteractAreaComponent InteractArea;
    [Export] public MeshInstance3D Mesh;
    [Export] public RatDef RatDef;
    [Export] public RatFlightTuning FlightTuning;
    [Export] public MeshInstance3D StatusMesh;
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
            if (_fsm is null) return;
            _debug = value;
            _fsm.Debug = _debug;
        }
    }
    private bool _debug;
    public Inventory Cargo;
    public Vector3 HomePosition;
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
        Cargo.Changed += CheckInventory;
        InitStateMachine();

        InteractArea.OnInteract = OnInteract;
        InteractArea.OnLookedAt = OnLookedAt;
        InteractArea.OnLookedAwayFrom = OnLookedAwayFrom;
    }

    private void CheckInventory()
    {
        if (Cargo.IsEmpty)
        {
            StatusMesh.Hide();
        }
        else
        {
            StatusMesh.Show();
        }
    }

    private void OnInteract(Node3D interactor, bool _)
    {
        if (interactor is not Player player) return;
        player.GrabComponent.InjectGrabState(this);
    }

    private void OnLookedAt()
    {
        if (Mesh.MaterialOverlay is ShaderMaterial mat)
        {
            mat.SetShaderParameter("outline_width", 2.5f);
        }
    }

    private void OnLookedAwayFrom()
    {
        if (Mesh.MaterialOverlay is ShaderMaterial mat)
        {
            mat.SetShaderParameter("outline_width", 0f);
        }
    }

    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);
    public override void _Process(double delta) => _fsm.StateProcess((float)delta);
    public T GetState<T>() where T : RatState
    {
        return _fsm.Get<T>();
    }

    public void ChangeState<T>() where T : RatState
    {
        _fsm.ChangeState<T>();
    }

    private void InitStateMachine()
    {
        _fsm = new FiniteStateMachine(this);
        _fsm.Add(new RatFollowState(this, NavAgent));
        _fsm.Add(new RatIdleState(this));
        _fsm.Add(new RatFallingState(this));
        _fsm.Add(new RatLandedState(this));
        _fsm.Add(new RatSlottedState(this));
        _fsm.Add(new RatIntakeState(this));
        _fsm.Add(new RatGrabState(this));
        _fsm.Add(new RatCurveState(this));
        _fsm.InitState<RatIdleState>();
        _fsm.Debug = _debug;
    }

    public void SetNavAgentEnabled(bool enabled)
    {
        NavAgent.SetProcess(enabled);
        NavAgent.SetPhysicsProcess(enabled);
    }

    public void ResetTargetPosition()
    {
        NavigationTargetPosition = HomePosition;
    }
}
