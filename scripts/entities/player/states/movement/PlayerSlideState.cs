using Godot;

public class PlayerSlideState : PlayerState
{
    const float SLIDE_DECAY = 0.95f;
    const float MAX_SLIDE_SPEED = 20f;
    const float SLIDE_VELOCITY_BOOST = 1.35f;
    const float SLIDE_EXIT_VELOCITY = 1f;
    private Vector3 _currentSlideVelocity;

    public PlayerSlideState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        _player.Velocity = _currentSlideVelocity + _player.GetGravity() * delta;
        // _player.MoveAndSlide();

        if (_player.IsOnFloor() || _player.GetFloorAngle() != 0)
        {
            if (_player.Velocity.Y >= 0)
                _currentSlideVelocity *= SLIDE_DECAY;
            else
                _currentSlideVelocity += _player.GetGravity() * delta;
        }

        if (_currentSlideVelocity.Length() < SLIDE_EXIT_VELOCITY)
        {
            fsm.ChangeState<PlayerIdleState>();
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            fsm.ChangeState<PlayerJumpState>(this);
            _player.CrouchComponent.TryStand();
        }
    }

    public override void Enter(State previous = null)
    {
        _currentSlideVelocity = _player.Velocity * SLIDE_VELOCITY_BOOST;
        _player.CrouchComponent.Crouch();
        _player.CrouchComponent.Enabled = false;
    }

    public override void Exit()
    {
        _player.CrouchComponent.Enabled = true;
    }
}
