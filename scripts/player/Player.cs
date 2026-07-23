using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public PlayerCamera Camera;
    [Export] public RayCast3D VaultRaycast;
    [Export] public ThrowComponent ThrowComponent;
    [Export] public float Speed = 10f;
    [Export] public float SprintSpeed = 15f;
    [Export] public float CrouchSpeed = 5f;
    [Export] public float Acceleration = 10f;
    [Export] public float AirAcceleration = 6f;
    [Export] public float Friction = 20f;
    [Export] public float JumpForce = 10f;

    public Vector3 Gravity;
    public GrabComponent GrabComponent;
    private FiniteStateMachine _movementFsm;
    private FiniteStateMachine _handFsm;

    public override void _Ready()
    {
        GrabComponent = new(this);
        Gravity = GetGravity();
        InitStateMachines();
    }

    public override void _Process(double delta)
    {
        _movementFsm.StateProcess((float)delta);
        _handFsm.StateProcess((float)delta);
    }
    public override void _PhysicsProcess(double delta)
    {
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
        _movementFsm.InitState("falling");
        _movementFsm.Debug = true;

        _handFsm = new(this);
        _handFsm.Add("empty", new HandEmptyState(this));
        _handFsm.Add("grab", new HandGrabState(this));
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

    public Vector3 GetMovementInputVelocity(float acceleration, float delta, float speedOverride = 0)
    {
        Vector2 input = GetInputVector();

        if (input == Vector2.Zero)
            return Vector3.Zero;

        float speed;
        if (speedOverride != 0) speed = speedOverride;
        else if (Input.IsActionPressed("sprint")) speed = SprintSpeed;
        else speed = Speed;

        float yaw = Rotation.Y;

        Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
        Vector3 right = new(forward.Z, 0, -forward.X);

        Vector3 desired =
            (right * input.X + forward * input.Y).Normalized();

        Vector3 velocity = Velocity;
        Vector3 horizontal = new(velocity.X, 0, velocity.Z);

        // Smaller acceleration in the air
        horizontal = horizontal.MoveToward(
            desired * speed,
            acceleration * delta);

        velocity.X = horizontal.X;
        velocity.Z = horizontal.Z;

        return velocity;
    }
}
