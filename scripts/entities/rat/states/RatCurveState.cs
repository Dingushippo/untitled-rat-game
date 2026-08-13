using Godot;
using System;

public class RatCurveState : RatState
{
    public ThrowTarget Target;
    public WorkSlot WorkSlot => Target.IsSlot ? Target.WorkSlot : null;

    private Vector3[] _pathArray;
    private float[] _speeds;
    private float[] _distanceToEnd;
    private RatFlightTuning _tuning;
    private Vector3 _exitVelocity;
    private int _currentIndex = 0;

    public RatCurveState(Rat owner) : base(owner) { }

    public void Configure(ThrowPath path)
    {
        _pathArray = path.Points;
        _speeds = path.Speeds;
        _exitVelocity = path.ExitVelocity;
        Target = path.ThrowTarget;
        _tuning = _rat.FlightTuning;

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
            if (Target.IsSlot) fsm.ChangeState<RatSlottedState>(this);
            else if (Target.IsIntake) fsm.ChangeState<RatIntakeState>(this);
            else if (!IsGrounded())
            {
                _rat.Velocity = _exitVelocity;
                fsm.ChangeState<RatFallingState>(this);
            }
            else fsm.ChangeState<RatLandedState>(this);
            return;
        }

        if (_rat.Collider.Disabled && RaycastUtils.Shape(_rat, _rat.Collider, out _, PhysicsLayers.GetOrMask(PhysicsLayers.WORLD, PhysicsLayers.FACILITY), false))
        {
            _rat.Collider.Disabled = false;
        }

        Advance(delta);

        // Advance can consume the last point; the state change happens next frame.
        if (_currentIndex >= _pathArray.Length)
            return;

        UpdateRotation(delta);
    }

    /// <summary>
    /// Walks the polyline over <paramref name="time"/> seconds using each segment's own simulated
    /// speed, so the rat actually slows toward the apex, accelerates on the way down, and leaves a
    /// bounce slower than it arrived. Advancing by a fixed distance instead would discard all of
    /// that and play every arc back at one flat speed.
    /// </summary>
    private void Advance(float time)
    {
        while (time > 0f && _currentIndex < _pathArray.Length)
        {
            Vector3 toTarget = _pathArray[_currentIndex] - _rat.GlobalPosition;
            float toTargetLength = toTarget.Length();

            if (toTargetLength < 0.0001f)
            {
                _rat.GlobalPosition = _pathArray[_currentIndex];
                _currentIndex++;
                continue;
            }

            float speed = SpeedAt(_currentIndex);
            float timeToPoint = toTargetLength / speed;

            if (timeToPoint <= time)
            {
                _rat.GlobalPosition = _pathArray[_currentIndex];
                time -= timeToPoint;
                _currentIndex++;
                continue;
            }

            _rat.GlobalPosition += toTarget / toTargetLength * speed * time;
            return;
        }
    }

    private float SpeedAt(int index)
    {
        float simulated = _speeds != null && index < _speeds.Length ? _speeds[index] : _tuning.MinSpeed;
        return Mathf.Max(simulated * _tuning.SpeedScale, _tuning.MinSpeed);
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
        if (Target.IsSlot)
        {
            float blend = 1f - Mathf.Clamp(_distanceToEnd[_currentIndex] / _tuning.ApproachBlendDistance, 0f, 1f);
            targetYaw = Mathf.LerpAngle(targetYaw, Target.WorkSlot.GlobalRotation.Y, blend);
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

    private bool IsGrounded()
    {
        float probeLength = _rat.FlightTuning.GroundProbeDistance;
        uint collisionMask = PhysicsLayers.WORLD;
        if (RaycastUtils.Ray(_rat, _rat.GlobalPosition, _rat.GlobalPosition + Vector3.Down * probeLength, out _, collisionMask, collideWithAreas: false))
        {
            return true;
        }
        return false;
    }

    public override void Exit()
    {
        EventBus.Publish(Event.RatLanded);
    }

}