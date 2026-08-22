using Godot;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (!_player.IsOnFloor())
        {
            fsm.ChangeState<PlayerFallingState>(this);
            return;
        }
        _player.Direction = _player.GetCorrectedInput();

        if (_player.LinearVelocity == Vector3.Zero && _player.Direction == Vector3.Zero)
        {
            fsm.ChangeState<PlayerIdleState>();
            return;
        }

        _player.Acceleration =
            _player.Direction == Vector3.Zero
                ? _player.Tuning.Acceleration
                : _player.Tuning.Deceleration;

        if (_player.CrouchComponent.IsCrouching)
            _player.Speed = _player.Tuning.CrouchSpeed;
        else if (Input.IsActionPressed("sprint"))
            _player.Speed = _player.Tuning.SprintSpeed;
        else
            _player.Speed = _player.Tuning.Speed;
    }

    public override void Enter(State previous = null) { }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            fsm.ChangeState<PlayerJumpState>(this);
        }
        if (@event.IsActionPressed("sprint"))
        {
            _player.CrouchComponent.TryStand();
        }
        if (
            @event.IsActionPressed("crouch")
            && Input.IsActionPressed("sprint")
            && !_player.CrouchComponent.IsCrouching
        )
        {
            fsm.ChangeState<PlayerSlideState>(this);
        }
        if (@event.IsActionPressed("crouch") && _player.IsOnFloor() && _player.GetFloorAngle() != 0)
        {
            if (_player.Velocity.Y < 0)
                fsm.ChangeState<PlayerSlideState>(this);
        }
    }
}
