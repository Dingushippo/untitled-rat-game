using Godot;
using Godot.Collections;

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
    public float VerticalAccel = 100;
    public Vector3 Velocity = Vector3.Zero;
    public Vector3 Direction;
    public bool IsOnFloor;
    public bool IsOnSlope;
    public Vector3 FloorNormal;
    public bool IsOnWall;
    public Vector3 WallNormal;
    public bool StickToFloor;

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
        CheckOnWall(state);
        HandleDirectionalMovement(state);
        HandleImpulse(state);
    }

    private void HandleImpulse(PhysicsDirectBodyState3D state)
    {
        if (!_wantsImpulse)
            return;
        state.ApplyCentralImpulse(_impulse);
        _wantsImpulse = false;
    }

    private void HandleDirectionalMovement(PhysicsDirectBodyState3D state)
    {
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

        if (IsOnFloor && StickToFloor)
        {
            if (IsOnSlope && FloorNormal.Y > 0.001f)
            {
                // Mathematical plane equation: forces the Y velocity to exactly match
                // whatever the current X and Z velocities are doing on the slope.
                currentVelocity.Y =
                    -(currentVelocity.X * FloorNormal.X + currentVelocity.Z * FloorNormal.Z)
                    / FloorNormal.Y;
            }
            else
                currentVelocity.Y = 0; // flat ground
        }
        state.LinearVelocity = currentVelocity;
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
        _movementFsm.Debug = false;

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
        float sideToSideScaling = 1f,
        Vector2 input = new()
    )
    {
        input = input == Vector2.Zero ? GetInputVector() : input;
        float yaw = Head.Rotation.Y;
        Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
        Vector3 right = new(forward.Z, 0, -forward.X);
        Vector3 desired = (
            right * input.X * sideToSideScaling + forward * input.Y * forwardBackwardScaling
        ).Normalized();
        return new(desired.X, 0, desired.Z);
    }

    public void CheckOnWall(PhysicsDirectBodyState3D state)
    {
        for (int i = 0; i < state.GetContactCount(); i++)
        {
            Vector3 contactNormal = state.GetContactLocalNormal(i);
            if (contactNormal != Vector3.Up && contactNormal != Vector3.Down)
            {
                float slopeAngleDegrees = Mathf.RadToDeg(Vector3.Up.AngleTo(FloorNormal));
                if (slopeAngleDegrees > Tuning.MaxWalkableSlopeDegrees || slopeAngleDegrees == 0)
                {
                    IsOnWall = true;
                    return;
                }
            }
        }
        IsOnWall = false;
    }

    public void CheckOnFloor(PhysicsDirectBodyState3D state)
    {
        if (!StickToFloor && (_wantsImpulse || state.LinearVelocity.Y > 0.1f))
        {
            IsOnFloor = IsOnSlope = false;
            FloorNormal = Vector3.Zero;
            return;
        }
        Vector3 startPos = GlobalPosition + Vector3.Up; // player height is 2 m, this starts on center

        float snapDistance = (IsOnFloor && StickToFloor) ? 1.4f : 1.1f; // variable overhead
        Vector3 endPos = startPos + Vector3.Down * snapDistance;
        if (!RaycastUtils.Ray(this, startPos, endPos, out Dictionary result, PhysicsLayers.WORLD))
        {
            IsOnFloor = IsOnSlope = false;
            FloorNormal = Vector3.Zero;
            return;
        }
        IsOnFloor = true;
        FloorNormal = result["normal"].AsVector3();

        // fix toe stubbing
        for (int i = 0; i < state.GetContactCount(); i++)
        {
            Vector3 contactNormal = state.GetContactLocalNormal(i);
            if (contactNormal.Y > 0.3f && contactNormal.Y < FloorNormal.Y)
                FloorNormal = contactNormal;
        }

        // check against max slope degrees
        float slopeAngleDegrees = Mathf.RadToDeg(Vector3.Up.AngleTo(FloorNormal));
        if (slopeAngleDegrees > Tuning.MaxWalkableSlopeDegrees)
        {
            IsOnFloor = false;
            IsOnSlope = false;
            WallNormal = FloorNormal;
            FloorNormal = Vector3.Zero;
            return;
        }

        IsOnSlope = FloorNormal != Vector3.Up;
        IsOnFloor = true;
    }

    public float GetFloorAngle()
    {
        return 0; // TODO implement
    }
}
