using Godot;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player owner) : base(owner) { }

    private Vector2 _inputDir;

    public override void PhysicsProcess(float delta)
    {
        if (!_player.IsOnFloor())
        {
            fsm.ChangeState("falling", this);
            return;
        }

        HandleMovement(delta);

        _player.MoveAndSlide();

        if (new Vector2(_player.Velocity.X, _player.Velocity.Z).Length() < 0.05f && _inputDir == Vector2.Zero)
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
        float acceleration = _player.GetInputVector() == Vector2.Zero ? _player.Friction : _player.Acceleration;
        Vector3 velocity = _player.GetMovementInputVelocity(acceleration, delta);
        _player.Velocity = velocity;
    }
}