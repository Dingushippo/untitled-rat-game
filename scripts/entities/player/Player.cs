using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public PlayerCamera Camera;
    [Export] public CollisionShape3D Collider;
    [Export] public RayCast3D VaultRaycast;
    [Export] public ThrowComponent ThrowComponent;
    [Export] public float Speed = 10f;
    [Export] public float SprintSpeed = 15f;
    [Export] public float CrouchSpeed = 5f;
    [Export] public float Acceleration = 55f;
    [Export] public float Deceleration = 90f;
    [Export] public float AirAcceleration = 25f;
    [Export] public float AirDeceleration = 0f;

    // How much harder we accelerate when the input fights the current velocity.
    // 1 = no extra bite on turns, higher = snappier direction changes.
    [Export] public float TurnBrakeMultiplier = 2.5f;
    [Export] public float JumpForce = 10f;

    public GrabComponent GrabComponent;
    public CrouchComponent CrouchComponent;
    public InteractComponent InteractComponent;
    public RitualComponent RitualComponent;
    private FiniteStateMachine _movementFsm;
    private FiniteStateMachine _handFsm;

    private bool _isFrozen = false;
    public override void _Ready()
    {
        GrabComponent = new(this);
        CrouchComponent = new(this);
        InteractComponent = new(this);
        RitualComponent = new(this);
        InitStateMachines();

        EventBus.Subscribe(Event.QteStarted, SetFrozen);
        EventBus.Subscribe(Event.QteCompleted, SetUnfrozen);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.QteStarted, SetFrozen);
        EventBus.Unsubscribe(Event.QteCompleted, SetUnfrozen);
    }

    private void SetFrozen(object[] _)
    {
        GD.Print("Set frozen");
        _movementFsm.SetEnabled(false);
        _handFsm.SetEnabled(false);
        Camera.SetCameraInputEnabled(false);
    }

    private void SetUnfrozen(object[] _)
    {
        GD.Print("Set unfrozen");
        _movementFsm.SetEnabled(true);
        _handFsm.SetEnabled(true);
        Camera.SetCameraInputEnabled(true);
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
        _movementFsm.Add("idle", new PlayerIdleState(this));
        _movementFsm.Add("move", new PlayerMoveState(this));
        _movementFsm.Add("jump", new PlayerJumpState(this));
        _movementFsm.Add("falling", new PlayerFallingState(this));
        _movementFsm.Add("vault", new PlayerVaultState(this));
        _movementFsm.Add("slide", new PlayerSlideState(this));
        _movementFsm.InitState("idle");
        _movementFsm.Debug = false;

        _handFsm = new(this);
        _handFsm.Add("empty", new HandEmptyState(this));
        _handFsm.Add("grab", new HandGrabState(this));
        _handFsm.Add("ritual", new HandRitualState(this));
        _handFsm.InitState("empty");
        _handFsm.Debug = false;
    }

    public Vector2 GetInputVector()
    {
        return Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back"
        );
    }

    public Vector3 GetMovementInputVelocity(float acceleration, float deceleration, float delta, float speedOverride = 0)
    {
        Vector2 input = GetInputVector();

        float speed;
        if (speedOverride != 0) speed = speedOverride;
        else if (Input.IsActionPressed("sprint")) speed = SprintSpeed;
        else speed = Speed;

        float yaw = Rotation.Y;

        Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
        Vector3 right = new(forward.Z, 0, -forward.X);

        Vector3 desired = (right * input.X + forward * input.Y).LimitLength() * speed;

        Vector3 velocity = Velocity;
        Vector3 horizontal = new(velocity.X, 0, velocity.Z);

        float rate;
        if (desired.IsZeroApprox())
        {
            // No input: brake. In air this is 0, so momentum is preserved.
            rate = deceleration;
        }
        else
        {
            // Input held: scale up the rate as the desired direction turns away from
            // the current velocity, so reversing doesn't have to coast through zero.
            float alignment = horizontal.IsZeroApprox()
                ? 1f
                : desired.Normalized().Dot(horizontal.Normalized());

            rate = acceleration * Mathf.Lerp(TurnBrakeMultiplier, 1f, (alignment + 1f) * 0.5f);
        }

        horizontal = horizontal.MoveToward(desired, rate * delta);

        velocity.X = horizontal.X;
        velocity.Z = horizontal.Z;

        return velocity;
    }
}
