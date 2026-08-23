using Godot;

public class PlayerArcMovementState : PlayerState
{
    public PlayerArcMovementState(Player owner)
        : base(owner) { }

    private Vector3 _targetPosition;
    private float _arcHeight;
    private float _moveDuration;

    private float _timer = 0f;
    private Vector3 _startPosition;

    public void Configure(Vector3 target, float arcHeight, float speed)
    {
        _targetPosition = target;
        _arcHeight = arcHeight;

        // Calculate how far we are traveling flat along the ground
        float horizontalDistance = _player.GlobalPosition.DistanceTo(target);
        // Time = Distance / Speed
        _moveDuration = horizontalDistance / speed;

        GD.Print(
            $"Configured target: {_targetPosition}, arc height: {_arcHeight}, duration: {_moveDuration}"
        );
    }

    public override void Enter(State previous = null)
    {
        _startPosition = _player.GlobalPosition;
        _timer = 0;

        GD.Print($"startPosition: {_startPosition}");
    }

    public override void IntegrateForces(PhysicsDirectBodyState3D state)
    {
        _timer += state.Step;

        float linearWeight = Mathf.Clamp(_timer / _moveDuration, 0.0f, 1.0f);
        float inverse = 1.0f - linearWeight;
        float weight = 1.0f - (inverse * inverse * inverse);

        // lerp between start and target
        Vector3 currentLinear = _startPosition.Lerp(_targetPosition, weight);

        // add arc offset
        float arc = 4.0f * _arcHeight * weight * (1.0f - weight);
        Vector3 arcOffset = Vector3.Up * arc;
        Vector3 targetPoint = currentLinear + arcOffset;

        Vector3 distanceToTarget = targetPoint - state.Transform.Origin;

        Vector3 desiredVelocity = distanceToTarget / (float)state.Step;
        state.LinearVelocity = desiredVelocity;

        if (weight >= 1.0)
        {
            Transform3D t = state.Transform;
            t.Origin = _targetPosition;
            state.Transform = t;

            GD.Print($"Endpos: {state.Transform.Origin}");
            fsm.ChangeState<PlayerFallingState>(this);
        }
    }
}
