using Godot;

public class PlayerSwingState : PlayerState
{
    private const float MAX_FORCE = 5f;
    private float _startWhipLength;
    private float _currentWhipLength => (_anchor - _player.GlobalPosition).Length();
    private float _currentRestLength;

    private readonly float _retractSpeed = 4f;
    private Vector3 _anchor;
    private Vector3 _initialSwingBasis;
    private Vector3 _whipVector => _anchor - _player.GlobalPosition;
    private Vector3 _startSwingPosition;

    public PlayerSwingState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (_currentWhipLength > _currentRestLength + 0.1f)
            MoveToRestVelocity(delta);
        else
            ApplySwingForce(delta);

        // _player.MoveAndSlide();
    }

    public override void Enter(State previous = null)
    {
        _anchor = _player.Whip.AnchorPoint;
        _startWhipLength = (_anchor - _player.GlobalPosition).Length();
        _currentRestLength = _startWhipLength - RatWhipComponent.WHIP_RETRACTION;
        _initialSwingBasis = _player.GlobalBasis.X;
        _startSwingPosition = _player.GlobalPosition;

        GD.Print(
            $"Start length: {_startWhipLength}, calculated: {_whipVector.Length()},rest length: {_currentRestLength}"
        );
    }

    public override void Exit()
    {
        // Todo change
        _player.Velocity = Vector3.Zero;
    }

    private Vector3 _swingDir = Vector3.Zero;

    private float _swingForce = 8f;
    private float _maxSwingForce = 10f;
    private float _angularVelocity = 0f;
    private float _angle;

    private void ApplySwingForce(float delta)
    {
        Vector3 testPos = (_player.HandL.GlobalPosition + _player.Velocity) * delta;

        if (_currentWhipLength > _currentRestLength)
        {
            GD.Print($"Current length: {_currentWhipLength}, rest: {_currentRestLength}");
            testPos = _whipVector.Normalized() * _currentRestLength;
        }
        _player.Velocity = (testPos - _player.HandL.GlobalPosition);
        // float vertical = _anchor.Y - _player.HandL.GlobalPosition.Y;
        // _angle = Mathf.Acos(vertical / _currentWhipLength);
        // float angularAccel = -(_player.GetGravity().Y / _currentWhipLength) * Mathf.Sin(_angle);
        // _angularVelocity += angularAccel * 0.99f * delta;
    }

    private void MoveToRestVelocity(float delta)
    {
        float force = _retractSpeed * (_currentWhipLength - _currentRestLength);

        _player.Velocity = _whipVector.Normalized() * force;
    }

    private void TweenPlayerAngle() { }
}
