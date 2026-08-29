using Godot;

public partial class PlayerSlideState : PlayerState
{
    [Export] public float SlideImpulse = 12.0f;
    [Export] public float Friction = 4.0f;
    [Export] public float MinSlideSpeed = 2.0f;

    private Vector3 _slideDirection;
    private float _currentSpeed;

    public override void Enter(State previous = null)
    {
        base.Enter(previous);

        // Lock initial direction and inherit momentum
        _slideDirection = _player.Input.Direction != Vector3.Zero
            ? _player.Input.Direction
            : -_player.Transform.Basis.Z;

        _currentSpeed = Mathf.Max(_player.Velocity.Length(), SlideImpulse);
    }

    public override void PhysicsProcess(float delta)
    {
        base.PhysicsProcess(delta);

        // Decelerate slide speed over time
        _currentSpeed = Mathf.MoveToward(_currentSpeed, 0f, Friction * delta);

        Vector3 newVelocity = _slideDirection * _currentSpeed;
        newVelocity.Y = _player.Velocity.Y;
        SetVelocity(newVelocity);
        MoveAndSlide();

        // Hand control back to ground matrix when speed drops
        if (_currentSpeed <= MinSlideSpeed)
        {
            if (_player.Input.WantsCrouch)
                _hfsm.ChangeState<PlayerCrouchState>();
            else if (_player.Input.Direction != Vector3.Zero)
                _hfsm.ChangeState<PlayerRunState>();
            else
                _hfsm.ChangeState<PlayerIdleState>();
        }
    }
}