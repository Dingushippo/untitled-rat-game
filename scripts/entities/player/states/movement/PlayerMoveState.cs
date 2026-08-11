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

        _inputDir = _player.GetInputVector();

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
        if (@event.IsActionPressed("sprint"))
        {
            _player.CrouchComponent.TryStand();
        }
        if (@event.IsActionPressed("crouch") && Input.IsActionPressed("sprint") && !_player.CrouchComponent.IsCrouching)
        {
            fsm.ChangeState("slide");
        }
        if (@event.IsActionPressed("crouch") && _player.IsOnFloor() && _player.GetFloorAngle() != 0)
        {
            if (_player.GetRealVelocity().Y < 0)
                fsm.ChangeState("slide");
        }

        // _player.IsOnFloor() || _player.GetFloorAngle() != 0
    }

    protected virtual void HandleMovement(float delta)
    {
        float speedOverride = _player.CrouchComponent.IsCrouching ? _player.CrouchSpeed : 0;
        _player.Velocity = _player.GetMovementInputVelocity(
            _player.Acceleration,
            _player.Deceleration,
            delta,
            speedOverride);
    }
}