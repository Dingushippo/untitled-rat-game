using Godot;

public class PlayerSlideState : PlayerState
{
    const float SLIDE_DECAY = 0.98f;
    const float MAX_SLIDE_SPEED = 20f;
    const float SLIDE_VELOCITY_BOOST = 2f;
    const float SLIDE_EXIT_VELOCITY = 1f;

    // private Vector3 _currentSlideVelocity;
    private float _currentSlideSpeed;

    public PlayerSlideState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        Vector2 inputDirection = _player.GetInputVector();
        inputDirection.Y = -1;

        _player.Direction = _player.GetCorrectedInput(
            input: inputDirection,
            sideToSideScaling: 0.2f
        );
        _player.HorizontalSpeed = _currentSlideSpeed;
        if (_player.IsOnFloor || _player.GetFloorAngle() != 0)
        {
            if (_player.LinearVelocity.Y >= 0)
                _currentSlideSpeed *= SLIDE_DECAY;
        }

        if (_currentSlideSpeed < SLIDE_EXIT_VELOCITY)
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
        _currentSlideSpeed = _player.HorizontalSpeed * SLIDE_VELOCITY_BOOST;
        _player.CrouchComponent.Crouch();
        _player.CrouchComponent.Enabled = false;
    }

    public override void Exit()
    {
        _player.CrouchComponent.Enabled = true;
    }
}
