using Godot;

[GlobalClass]
public partial class Player : CharacterBody3D
{
    [Export] public PlayerCamera Camera;
    [Export] public Node3D HandL;
    [Export] public Node3D HandR;
    [Export] public SpellData CurrentSpell;

    [Export] public Node3D Head;
    [Export] public InputComponent InputComponent;
    [Export] public AbilityManager AbilityManager;
    [Export] public CastManager CastManager;

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
        InputComponent.Init(this);
        _movementFsm.Init(this);
        // _handFsm.Init(this);

        EventBus.Subscribe<QteStarted>(OnQteStarted);
        EventBus.Subscribe<QteCompleted>(OnQteCompleted);
    }

    public bool IsMovementState<T>()
        where T : PlayerState
    {
        return _movementFsm.IsState<T>();
    }

    public bool IsMovementStateBranch<T>()
        where T : PlayerState
    {
        return _movementFsm.IsStateBranch<T>();
    }

    public float Speed => Velocity.Length();

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
        DebugDraw.Clear(); // Feels like a good place to have it

        if (InputComponent.LeftArmAction)
        {
            AbilityManager.ActivateAbility();
        }
        if (Input.IsActionJustPressed("right_hand"))
        {
            Vector3 target = HandR.GlobalPosition - Camera.GlobalBasis.Z;
            DebugDraw.Sphere(this, target, 0.3f, Colors.Orange);
            GD.Print($"Target: {target}");
            CastManager.Cast(CurrentSpell, target);
        }
        InteractComponent.PhysicsUpdate((float)delta);
    }
}
