using Godot;

public class RatFollowState : RatState
{
    public RatFollowState(Rat owner, NavigationAgent3D navAgent) : base(owner) { _navAgent = navAgent; }


    private NavigationAgent3D _navAgent;
    private Vector3 _navigationVelocity;
    private Vector3 _intendedDirection;
    private float _currentSpeed = 0;
    private float _desiredSpeed = 0;
    private float _acceleration = 0;



    #region State methods
    public override void PhysicsProcess(float delta)
    {
        if (_navAgent.IsTargetReached())
        {
            fsm.ChangeState("target_reached", this);
            return;
        }
        Vector3 nextPoint = _navAgent.GetNextPathPosition();
        Vector3 direction = _rat.GlobalPosition.DirectionTo(nextPoint);
        // direction.Y = 0;
        if (direction.LengthSquared() < 0.001f)
        {
            // Feed zero velocity to the agent
            _navAgent.Velocity = Vector3.Zero;
            _intendedDirection = Vector3.Zero;
            return;
        }
        _intendedDirection = direction.Normalized();
        _currentSpeed = Mathf.MoveToward(_currentSpeed, _desiredSpeed, _acceleration * delta);
        Vector3 desiredVelocity = _intendedDirection * _currentSpeed;

        RotateTowardsDirection(_intendedDirection, delta);

        // Set nav agent velocity, to trigger UpdateNavigationVelocity,
        // which in turn sets the safe velocity
        _navAgent.Velocity = desiredVelocity;
        _rat.Velocity = _navigationVelocity;
        _rat.MoveAndSlide();
    }

    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        _navAgent.VelocityComputed += UpdateNavigationVelocity;
        _navAgent.TargetPosition = _rat.NavigationTargetPosition;
        _desiredSpeed = _rat.Speed;
        _acceleration = _rat.Acceleration;
    }
    public override void Exit()
    {
        _navAgent.VelocityComputed -= UpdateNavigationVelocity;
    }

    #endregion

    private void UpdateNavigationVelocity(Vector3 safeVelocity)
    {
        _navigationVelocity = safeVelocity;
    }

    private void RotateTowardsDirection(Vector3 direction, float delta)
    {
        // May need to change this if implementing slopes
        Vector3 flatDirection = new Vector3(direction.X, 0, direction.Z);
        if (flatDirection.LengthSquared() < 0.2f)
            return;

        float targetYaw = Mathf.Atan2(-flatDirection.X, -flatDirection.Z);

        Vector3 rot = _rat.Rotation;
        rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, 8f * delta);
        _rat.Rotation = rot;
    }

}