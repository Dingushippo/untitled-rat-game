using Godot;

public class PlayerFallingState : PlayerState
{
    public PlayerFallingState(Player owner) : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        HandleAirMovement(delta);

        _player.Velocity += _player.gravity * delta;

        _player.MoveAndSlide();

        if (_player.IsOnFloor())
        {
            fsm.ChangeState("move", this);
        }
    }

    private void HandleAirMovement(float delta)
    {
        Vector2 input = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back");

        if (input == Vector2.Zero)
            return;

        float speed = Input.IsActionPressed("sprint")
            ? _player.SprintSpeed
            : _player.Speed;

        float yaw = _player.Rotation.Y;

        Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
        Vector3 right = new(forward.Z, 0, -forward.X);

        Vector3 desired =
            (right * input.X + forward * input.Y).Normalized();

        Vector3 velocity = _player.Velocity;
        Vector3 horizontal = new(velocity.X, 0, velocity.Z);

        // Smaller acceleration in the air
        horizontal = horizontal.MoveToward(
            desired * speed,
            _player.AirAcceleration * delta);

        velocity.X = horizontal.X;
        velocity.Z = horizontal.Z;

        _player.Velocity = velocity;
    }
}