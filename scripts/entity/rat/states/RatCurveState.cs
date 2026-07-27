using Godot;

public class RatCurveState : RatState
{
    public WorkSlot WorkSlot = null;

    private readonly Vector3[] _pathArray;
    private readonly float[] _distanceToEnd;
    private readonly RatFlightTuning _tuning;
    private readonly float _speed;
    private int _currentIndex = 0;

    public RatCurveState(Rat owner, Vector3[] pathArray, float speed, WorkSlot slot = null) : base(owner)
    {
        _pathArray = pathArray;
        _speed = speed;
        WorkSlot = slot;
        _tuning = owner.FlightTuning;

        _distanceToEnd = new float[_pathArray.Length];
        for (int i = _pathArray.Length - 2; i >= 0; i--)
        {
            _distanceToEnd[i] = _distanceToEnd[i + 1] + _pathArray[i].DistanceTo(_pathArray[i + 1]);
        }
    }

    public override void PhysicsProcess(float delta)
    {
        if (_currentIndex >= _pathArray.Length)
        {
            string nextState;
            if (WorkSlot != null) nextState = "slotted";
            else if (!_rat.IsOnFloor()) nextState = "falling";
            else nextState = "landed";

            fsm.ChangeState(nextState, this);
            return;
        }

        Advance(_speed * delta);

        // Advance can consume the last point; the state change happens next frame.
        if (_currentIndex >= _pathArray.Length)
            return;

        UpdateRotation(delta);
    }

    /// <summary>Walks the polyline at a constant speed, consuming as many points as the step covers.</summary>
    private void Advance(float distance)
    {
        while (distance > 0f && _currentIndex < _pathArray.Length)
        {
            Vector3 toTarget = _pathArray[_currentIndex] - _rat.GlobalPosition;
            float toTargetLength = toTarget.Length();

            if (toTargetLength <= distance || toTargetLength < 0.0001f)
            {
                _rat.GlobalPosition = _pathArray[_currentIndex];
                distance -= toTargetLength;
                _currentIndex++;
                continue;
            }

            _rat.GlobalPosition += toTarget / toTargetLength * distance;
            return;
        }
    }

    private void UpdateRotation(float delta)
    {
        Vector3 direction = LookAheadPoint(_tuning.LookAheadDistance) - _rat.GlobalPosition;
        Vector3 flatDirection = new(direction.X, 0f, direction.Z);

        Vector3 rotation = _rat.Rotation;

        float targetYaw = flatDirection.LengthSquared() > 0.0001f
            ? Mathf.Atan2(-flatDirection.X, -flatDirection.Z)
            : rotation.Y;

        float targetPitch = Mathf.Clamp(
            Mathf.Atan2(direction.Y, flatDirection.Length()),
            -Mathf.DegToRad(_tuning.MaxPitchDegrees),
            Mathf.DegToRad(_tuning.MaxPitchDegrees)
        );

        // Settle into the slot's facing over the last stretch so the handoff isn't a snap.
        if (WorkSlot != null)
        {
            float blend = 1f - Mathf.Clamp(_distanceToEnd[_currentIndex] / _tuning.ApproachBlendDistance, 0f, 1f);
            targetYaw = Mathf.LerpAngle(targetYaw, WorkSlot.GlobalRotation.Y, blend);
            targetPitch = Mathf.Lerp(targetPitch, 0f, blend);
        }

        float weight = _tuning.TurnSpeed * delta;
        rotation.X = Mathf.LerpAngle(rotation.X, targetPitch, weight);
        rotation.Y = Mathf.LerpAngle(rotation.Y, targetYaw, weight);
        rotation.Z = Mathf.LerpAngle(rotation.Z, 0f, weight);
        _rat.Rotation = rotation;
    }

    /// <summary>Point roughly <paramref name="distance"/> further along the path, for a stable heading.</summary>
    private Vector3 LookAheadPoint(float distance)
    {
        Vector3 previous = _rat.GlobalPosition;
        float travelled = 0f;

        for (int i = _currentIndex; i < _pathArray.Length; i++)
        {
            travelled += previous.DistanceTo(_pathArray[i]);
            previous = _pathArray[i];

            if (travelled >= distance)
                return _pathArray[i];
        }

        return _pathArray[^1];
    }

    public override void Exit()
    {
        EventBus.Publish(Event.RatLanded);
    }

}