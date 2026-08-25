using Godot;

public class PlayerSwingState : PlayerState
{
    private Vector3 _anchorPoint => _player.Whip.AnchorPoint;
    private float _restLength => _player.Whip.RestLength;
    private bool _isAnchored => _player.Whip.IsAnchored;

    public PlayerSwingState(Player owner)
        : base(owner) { }

    public override void IntegrateForces(PhysicsDirectBodyState3D state)
    {
        _player.CheckOnFloor(state);

        if (!_isAnchored)
        {
            fsm.ChangeState<PlayerFallingState>(this);
        }

        Vector3 direction = _anchorPoint - _player.GlobalPosition;
        float currentLength = direction.Length();
        if (currentLength <= 0)
            return;
        Vector3 ropeDirection = direction.Normalized();
        float stretch = currentLength - _restLength;

        if (stretch > 0)
        {
            float relativeVelocity = state.LinearVelocity.Dot(ropeDirection);

            // apply hookes law
            float springForceMultiplier =
                (_player.Tuning.SpringStiffness * stretch)
                - (_player.Tuning.SpringDamping * relativeVelocity);
            Vector3 springForce = ropeDirection * springForceMultiplier;
            state.ApplyCentralForce(springForce);
        }

        // Handle swing
        Vector3 inputDir = _player.GetCorrectedInput();
        if (inputDir.Length() <= 0)
            return;

        Plane swingPlane = new Plane(ropeDirection);
        Vector3 tangentialDirection = swingPlane.Project(inputDir).Normalized();
        state.ApplyCentralForce(tangentialDirection * _player.Tuning.SwingForce);
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            _player.Whip.LaunchToAnchor();
        }
    }
}
