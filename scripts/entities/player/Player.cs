using Godot;

[GlobalClass]
public partial class Player : CharacterBody3D
{
    [Export] public PlayerCamera Camera;

    [Export] public Node3D HandL;

    [Export] public Node3D HandR;

    [Export] public Node3D Head;
    [Export] public InputComponent Input;

    [Export] public CollisionShape3D Collider;

    [Export] public RayCast3D VaultRaycast;

    [Export] public PlayerMovementTuning Tuning;

    // [Export] public RatWhipComponent Whip;
    public InteractComponent InteractComponent;

    [Export] private HierarchicalStateMachine _movementFsm;
    // [Export] private HierarchicalStateMachine _handFsm;

    public override void _Ready()
    {
        InteractComponent = new(this);
        Input.Init(this);
        _movementFsm.Init(this);
        // _handFsm.Init(this);

        EventBus.Subscribe<QteStarted>(OnQteStarted);
        EventBus.Subscribe<QteCompleted>(OnQteCompleted);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe<QteStarted>(OnQteStarted);
        EventBus.Unsubscribe<QteCompleted>(OnQteCompleted);
    }

    private void OnQteStarted(QteStarted _) => SetFrozen();

    private void OnQteCompleted(QteCompleted _) => SetUnfrozen();

    private void SetFrozen()
    {
        Camera.SetCameraInputEnabled(false);
    }

    private void SetUnfrozen()
    {
        Camera.SetCameraInputEnabled(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        InteractComponent.PhysicsUpdate((float)delta);
    }
}
