using Godot;

[GlobalClass]
public partial class PlayerMovementTuning : Resource
{
    [ExportGroup("Base acceleration")]
    [Export]
    public float Acceleration = 55f;

    [Export]
    public float Deceleration = 90f;

    [ExportGroup("Ground movement")]
    [Export]
    public float Speed = 10f;

    [Export]
    public float SprintSpeed = 15f;

    [Export]
    public float CrouchSpeed = 5f;

    [Export]
    public float FloorStickForce = 10f;

    [Export]
    public float FloorStickAccel = 10f;

    [Export]
    public float MaxWalkableSlopeDegrees = 45f;

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
    public float JumpAcceleration = 10f;

    [Export]
    public float SlideJumpBoost = 1.4f;

    [ExportGroup("Whip")]
    [Export]
    public float WhipMaxDistance = 20f;

    [Export]
    public float WhipSpringStiffness = 50f;

    [Export]
    public float WhipSpringDamping = 5.0f;

    [Export]
    public float WhipRestLengthMultiplier = 0.8f;

    [Export]
    public float WhipSwingForce = 10f;

    [ExportGroup("Whip Arc move")]
    [Export]
    public float ArcMinHeight = 1.5f;

    [Export]
    public float ArcHeightMultiplier = 1.5f;

    [Export]
    public float ArcMoveSpeed = 10f;

    [Export]
    public Tween.EaseType ArcEase = Tween.EaseType.Out;

    [Export]
    public Tween.TransitionType ArcTrans = Tween.TransitionType.Back;
}
