using Godot;

public partial class Player : RigidBody3D
{
    [Export]
    public PlayerCamera Camera;

    [Export]
    public Node3D HandL;

    [Export]
    public Node3D HandR;

    [Export]
    public Node3D Head;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public RayCast3D VaultRaycast;

    [Export]
    public PlayerMovementTuning Tuning;

    [Export]
    public RatWhipComponent Whip;

    public CrouchComponent CrouchComponent;
    public InteractComponent InteractComponent;

    public float HorizontalSpeed;
    public float VerticalSpeed;
    public float HorizontalAccel;
    public float VerticalAccel;
    public Vector3 Velocity = Vector3.Zero;
    public Vector3 Direction;
    public bool IsOnFloor;
    public bool IsOnWall;

    private FiniteStateMachine<PlayerState> _movementFsm;
    private FiniteStateMachine<HandState> _handFsm;

    private bool _isFrozen = false;
    private Vector3 _impulse = Vector3.Zero;
    private bool _wantsImpulse;

    public override void _Ready()
    {
        CrouchComponent = new(this);
        InteractComponent = new(this);
        InitStateMachines();

        LockRotation = true;

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
        _movementFsm.SetEnabled(false);
        Camera.SetCameraInputEnabled(false);
    }

    private void SetUnfrozen()
    {
        _movementFsm.SetEnabled(true);
        Camera.SetCameraInputEnabled(true);
    }

    public void SetImpulse(Vector3 impulse)
    {
        _impulse = impulse;
        _wantsImpulse = true;
    }

    public override void _Process(double delta)
    {
        CrouchComponent.Update();
        _movementFsm.StateProcess((float)delta);
        _handFsm.StateProcess((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        InteractComponent.PhysicsUpdate((float)delta);
        _movementFsm.StatePhysicsProcess((float)delta);
        _handFsm.StatePhysicsProcess((float)delta);
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
        CheckOnFloor(state);

        Vector3 targetVelocity = Direction * HorizontalSpeed;
        Vector3 currentVelocity = state.LinearVelocity;
        currentVelocity.X = Mathf.MoveToward(
            currentVelocity.X,
            targetVelocity.X,
            HorizontalAccel * state.Step
        );
        currentVelocity.Z = Mathf.MoveToward(
            currentVelocity.Z,
            targetVelocity.Z,
            HorizontalAccel * state.Step
        );
        currentVelocity.Y = Mathf.MoveToward(
            currentVelocity.Y,
            VerticalSpeed,
            VerticalAccel * state.Step
        );
        state.LinearVelocity = currentVelocity;

        if (_wantsImpulse)
        {
            state.ApplyCentralImpulse(_impulse);
            _wantsImpulse = false;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _movementFsm.StateUnhandledInput(@event);
        _handFsm.StateUnhandledInput(@event);
    }

    public override void _Input(InputEvent @event)
    {
        _movementFsm.StateInput(@event);
        _handFsm.StateInput(@event);
    }

    private void InitStateMachines()
    {
        _movementFsm = new(this);
        _movementFsm.Add(new PlayerIdleState(this));
        _movementFsm.Add(new PlayerMoveState(this));
        _movementFsm.Add(new PlayerJumpState(this));
        _movementFsm.Add(new PlayerFallingState(this));
        _movementFsm.Add(new PlayerVaultState(this));
        _movementFsm.Add(new PlayerSlideState(this));
        _movementFsm.Add(new PlayerWallRunState(this));
        _movementFsm.Add(new PlayerWallJumpState(this));
        _movementFsm.Add(new PlayerSwingState(this));
        _movementFsm.InitState<PlayerIdleState>();
        _movementFsm.Debug = true;

        _handFsm = new(this);
        _handFsm.Add(new HandEmptyState(this));
        _handFsm.InitState<HandEmptyState>();
        _handFsm.Debug = false;
    }

    public void ChangeMovementState<T>()
        where T : PlayerState
    {
        _movementFsm.ChangeState<T>();
    }

    public void ChangeHandState<T>()
        where T : HandState
    {
        _handFsm.ChangeState<T>();
    }

    public Vector2 GetInputVector()
    {
        return Input.GetVector("move_left", "move_right", "move_forward", "move_back");
    }

    public Vector3 GetCorrectedInput(
        float forwardBackwardScaling = 1f,
        float sideToSideScaling = 1f
    )
    {
        Vector2 input = GetInputVector();
        float yaw = Head.Rotation.Y;
        Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
        Vector3 right = new(forward.Z, 0, -forward.X);
        Vector3 desired = (
            right * input.X * sideToSideScaling + forward * input.Y * forwardBackwardScaling
        ).Normalized();
        return new(desired.X, 0, desired.Z);
    }

    public void CheckOnFloor(PhysicsDirectBodyState3D state)
    {
        if (state.GetContactCount() == 0)
        {
            IsOnFloor = false;
            return;
        }
        for (int i = 0; i < state.GetContactCount(); i++)
        {
            Vector3 localNormal = state.GetContactLocalNormal(i);
            if (localNormal.Dot(Vector3.Up) < 0.3f)
                continue;
            IsOnFloor = true;
            return;
        }
    }

    public float GetFloorAngle()
    {
        return 0; // TODO implement
    }
}
