using Godot;

[GlobalClass]
public partial class PlayerMovementTuning : Resource
{
    [ExportGroup("Base acceleration")]
    [Export]
    public float Acceleration = 55f;

    [Export]
    public float Deceleration = 90f;

    [Export]
    public PidTuning Pid;

    [ExportGroup("Ground movement")]
    [Export]
    public float Speed = 10f;

    [Export]
    public float SprintSpeed = 15f;

    [Export]
    public float CrouchSpeed = 5f;

    [ExportGroup("Wallrun")]
    [Export]
    public float WallrunSpeed = 12f;

    [Export]
    public float WallrunGravityScale = 0.9f;

    [Export]
    public float WallJumpForce = 8f;

    [Export]
    public float WallrunCheckDistance = 1f;

    [ExportGroup("Arial")]
    [Export]
    public float AirAcceleration = 25f;

    [Export]
    public float AirDeceleration = 0f;

    [Export]
    public float TurnBrakeMultiplier = 2.5f;

    [Export]
    public float JumpForce = 10f;

    [Export]
    public float SlideJumpBoost = 1.4f;
}
