using Godot;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (!_player.IsOnFloor)
        {
            _hfsm.ChangeState<PlayerFallingState>(this);
            return;
        }
        _player.Direction = _player.GetCorrectedInput();

        if (_player.LinearVelocity == Vector3.Zero && _player.Direction == Vector3.Zero)
        {
            _hfsm.ChangeState<PlayerIdleState>();
            return;
        }

        _player.HorizontalAccel =
            _player.Direction == Vector3.Zero
                ? _player.Tuning.Acceleration
                : _player.Tuning.Deceleration;

        if (_player.CrouchComponent.IsCrouching)
            _player.HorizontalSpeed = _player.Tuning.CrouchSpeed;
        else if (Input.IsActionPressed("sprint"))
            _player.HorizontalSpeed = _player.Tuning.SprintSpeed;
        else
            _player.HorizontalSpeed = _player.Tuning.Speed;
    }

    public override void Exit() { }

    public override void Enter(State previous = null)
    {
        _player.StickToFloor = true;
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            _hfsm.ChangeState<PlayerJumpState>(this);
            return;
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
            _hfsm.ChangeState<PlayerSlideState>(this);
        }
        if (@event.IsActionPressed("crouch") && _player.IsOnFloor && _player.GetFloorAngle() != 0)
        {
            if (_player.Velocity.Y < 0)
                _hfsm.ChangeState<PlayerSlideState>(this);
        }
    }
}
