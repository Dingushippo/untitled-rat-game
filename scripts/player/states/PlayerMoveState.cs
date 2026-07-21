using Godot;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player owner) : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (!_player.IsOnFloor())
        {
            fsm.ChangeState("falling", this);
            return;
        }

        HandleMovement(delta);

        _player.MoveAndSlide();

        if (new Vector2(_player.Velocity.X, _player.Velocity.Z).Length() < 0.05f)
        {
            fsm.ChangeState("idle");
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            fsm.ChangeState("jump", this);
        }
    }

    private void HandleMovement(float delta)
    {
        Vector2 input = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back");

        float speed = Input.IsActionPressed("sprint")
            ? _player.SprintSpeed
            : _player.Speed;

        Vector3 velocity = _player.Velocity;

        Vector3 target = Vector3.Zero;

        if (input != Vector2.Zero)
        {
            float yaw = _player.Rotation.Y;

            Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
            Vector3 right = new(forward.Z, 0, -forward.X);

            Vector3 desired =
                (right * input.X + forward * input.Y).Normalized();

            target = desired * speed;
        }

        Vector3 horizontal = new(velocity.X, 0, velocity.Z);

        float accel = input == Vector2.Zero
            ? _player.Friction
            : _player.Acceleration;

        horizontal = horizontal.MoveToward(target, accel * delta);
        velocity.X = horizontal.X;
        velocity.Z = horizontal.Z;

        _player.Velocity = velocity;
    }
}