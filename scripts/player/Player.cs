using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public PlayerCamera Camera;
    [Export] public RayCast3D VaultRaycast;
    [Export] public ThrowAnchor ThrowAnchor;
    [Export] public float Speed = 10f;
    [Export] public float SprintSpeed = 15f;
    [Export] public float CrouchSpeed = 5f;
    [Export] public float Acceleration = 10f;
    [Export] public float AirAcceleration = 6f;
    [Export] public float Friction = 20f;
    [Export] public float JumpForce = 10f;

    public Vector3 gravity;
    private FiniteStateMachine _fsm;

    public override void _Ready()
    {
        gravity = GetGravity();
        InitStateMachine();
    }

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);
    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);
    public override void _UnhandledInput(InputEvent @event) => _fsm.StateUnhandledInput(@event);
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.Pressed)
            {
                EventBus.Publish(Event.MouseClick, mouseEvent.ButtonIndex, mouseEvent.Position);
            }
        }
        _fsm.StateInput(@event);
    }

    private void InitStateMachine()
    {
        _fsm = new FiniteStateMachine();
        _fsm.Add("idle", new PlayerIdleState(this));
        _fsm.Add("move", new PlayerMoveState(this));
        _fsm.Add("jump", new PlayerJumpState(this));
        _fsm.Add("falling", new PlayerFallingState(this));
        _fsm.Add("vault", new PlayerVaultState(this));
        _fsm.InitState("idle");
        _fsm.Debug = true;
    }
}
