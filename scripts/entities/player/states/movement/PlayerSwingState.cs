using Godot;

public class PlayerSwingState : PlayerState
{
    float _springStiffness = 50f;
    float _springDamping = 5.0f;
    float _restLengthMultiplier = 0.8f;
    float _swingForce = 10f;

    private Vector3 _anchorPoint;
    private float _restLength;

    public PlayerSwingState(Player owner)
        : base(owner) { }

    public override void IntegrateForces(PhysicsDirectBodyState3D state)
    {
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
                (_springStiffness * stretch) - (_springDamping * relativeVelocity);
            Vector3 springForce = ropeDirection * springForceMultiplier;
            state.ApplyCentralForce(springForce);
        }

        // Handle swing
        Vector3 inputDir = _player.GetCorrectedInput();
        if (inputDir.Length() <= 0)
            return;

        Plane swingPlane = new Plane(ropeDirection);
        Vector3 tangentialDirection = swingPlane.Project(inputDir).Normalized();
        state.ApplyCentralForce(tangentialDirection * _swingForce);
    }

    public override void Enter(State previous = null)
    {
        _anchorPoint = _player.Whip.AnchorPoint;
        _restLength = _player.GlobalPosition.DistanceTo(_anchorPoint) * _restLengthMultiplier;
    }
}
