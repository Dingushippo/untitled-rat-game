using Godot;

public class PlayerSlideState : PlayerState
{
    const float SLIDE_DECAY = 0.95f;
    const float SLIDE_VELOCITY_BOOST = 1.35f;
    const float SLIDE_EXIT_VELOCITY = 1f;
    private Vector3 _currentSlideVelocity;
    public PlayerSlideState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        _player.Velocity = _currentSlideVelocity;
        _player.MoveAndSlide();
        _currentSlideVelocity *= SLIDE_DECAY;

        if (_currentSlideVelocity.Length() < SLIDE_EXIT_VELOCITY)
        {
            fsm.ChangeState("idle");
        }
    }
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            fsm.ChangeState("jump", this);
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