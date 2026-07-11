using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class PathfindingComponent : NavigationAgent3D
{
	// Export variables
	[Export] public CharacterBody3D MovementTarget;
	[Export] public Node3D NavigationTarget;
	[Export] public float Speed = 5.0f;
	[Export] public float JumpStrength = 5.0f;
	[Export] public float GravityScale = 1.0f;
	[Export] public float Acceleration = 20.0f;
	[Export] public float Deceleration = 16.0f;
	[Export] public float StoppingDistance = 0.5f;


	// Private variables
	private Vector2 _direction { get; set; }
	private float gravity;
	private float currentSpeed = 0f;
	private bool _isEnabled = true;
	private Vector3 _navigationVelocity = Vector3.Zero;

	#region Built in godot methods
	public override void _Ready()
	{
		if (MovementTarget == null)
		{
			GD.PrintErr("PathfindingComponent requires a MovementTarget to function.");
		}

		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		VelocityComputed += UpdateNavigationVelocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Always track the target so the agent knows if it needs to wake up
		if (NavigationTarget != null)
		{
			Vector3 newTargetPos = NavigationTarget.GlobalPosition;
			if (TargetPosition.DistanceSquaredTo(newTargetPos) > 0.1f)
			{
				TargetPosition = newTargetPos;
			}
		}

		ProcessState((float)delta);

		if (_currentState == State.Disabled) return;

		ApplyMovement((float)delta);
		MovementTarget.MoveAndSlide();
		UpdateStateTransitions();
	}
	#endregion

	#region State machine

	public enum State
	{
		Idle,
		Following,
		Falling,
		Landed,
		ForceApplied,
		Disabled,
	}
	private State _currentState = State.Falling;
	private State _previousState;
	private float desiredSpeed = 0f;
	private bool isLanding = false;
	private void ProcessState(float delta)
	{
		switch (_currentState)
		{
			case State.Idle:
				IdleState(delta); break;
			case State.Following:
				FollowingState(delta); break;
			case State.Falling:
				FallingState(delta); break;
			case State.Landed:
				LandedState(delta); break;
			case State.ForceApplied:
				ForceAppliedState((float)delta); break;
		}
	}

	private void UpdateStateTransitions()
	{
		switch (_currentState)
		{
			case State.Idle:
			case State.Following:
				if (!MovementTarget.IsOnFloor())
					SetState(State.Falling);
				break;
			case State.Falling:
				if (MovementTarget.IsOnFloor())
					SetState(State.Landed);
				break;
			case State.ForceApplied:
				break;
			case State.Disabled:
				break;
		}
	}

	public void SetState(State newState)
	{
		_previousState = _currentState;
		_currentState = newState;
		GD.Print($"{GetParent().Name} switched from {_previousState} to {_currentState}");
	}
	private void DisabledState()
	{
		// Nothing here for now
	}
	private void IdleState(float delta)
	{
		desiredSpeed = 0f;

		if (isLanding)
			return;

		if (NavigationTarget != null)
		{
			SetState(State.Following);
		}
	}

	private void FollowingState(float delta)
	{
		TargetPosition = NavigationTarget.GlobalPosition;

		float distance = MovementTarget.GlobalPosition.DistanceTo(TargetPosition);

		if (distance <= StoppingDistance)
		{
			float t = Mathf.Clamp(distance / StoppingDistance, 0f, 1f);
			desiredSpeed = Mathf.Lerp(1f, Speed, t);
		}
		else
		{
			desiredSpeed = Speed;
		}
		if (IsNavigationFinished())
		{
			desiredSpeed = 0;
			_navigationVelocity = Vector3.Zero;
		}
	}

	private void FallingState(float delta)
	{
		// Nothing to do here yet
	}

	private void LandedState(float delta)
	{
		if (isLanding) return;

		isLanding = true;
		Vector3 landingDirection = new Vector3(
			0,
			MovementTarget.GlobalRotation.Y,
			0
		);	

		Tween tween = CreateTween();
		tween.TweenProperty(MovementTarget,	"rotation",	landingDirection, 0.35f);
		tween.TweenCallback(Callable.From(() =>
		{
			isLanding = false;
			SetState(NavigationTarget != null ? State.Following	: State.Idle);
		}));
	}

	private Vector3 rotateDir = Vector3.Zero;
	private float rotateSpeed = 0;
	private void ForceAppliedState(float delta)
	{
		if (MovementTarget.IsOnFloor())
		{
			SetState(State.Landed); return;
		}

		MovementTarget.Rotate(rotateDir, rotateSpeed * delta);
		GD.Print($"Rotate direction: {rotateDir}, current rotation: {MovementTarget.Rotation}");
	}

	public void ApplyForce(Vector3 force)
	{
		Godot.RandomNumberGenerator _rng = new Godot.RandomNumberGenerator();
		
		rotateDir = new Vector3(
			_rng.RandfRange(-1f, 1f),
			_rng.RandfRange(-1f, 1f),
			_rng.RandfRange(-1f, 1f)
		).Normalized();

		rotateSpeed = _rng.RandfRange(2f, 10f);

		MovementTarget.Velocity += force;
		SetState(State.ForceApplied);
	}
	#endregion

	#region Public methods
	public void EnablePathfinding()
	{
		// _isEnabled = true;
		SetState(State.Following);
	}

	public void DisablePathfinding()
	{
		SetState(State.Disabled);
		// _isEnabled = false;
		// SetMovementTargetVelocity(Vector3.Zero);
	}
	#endregion

	#region Private methods

	private Vector3 _intendedDirection = Vector3.Zero;
    private void ApplyMovement(float delta)
    {
        Vector3 velocity = MovementTarget.Velocity;

        // Smoothly accelerate/decelerate currentSpeed towards desiredSpeed
        currentSpeed = Mathf.MoveToward(currentSpeed, desiredSpeed, Acceleration * delta);

        if (_currentState == State.Following)
        {
            CalculateDesiredVelocity(); 
        }

        ApplyHorizontalMovement(ref velocity, delta);
        ApplyGravity(ref velocity, delta);
		// GD.Print($"Velocity: {velocity}");
        MovementTarget.Velocity = velocity;
        // MovementTarget.Velocity = Velocity;
    }

    // Renamed to avoid confusion with the VelocityComputed event handler
    private void CalculateDesiredVelocity() 
    {
        Vector3 nextPoint = GetNextPathPosition();
        Vector3 direction = MovementTarget.GlobalPosition.DirectionTo(nextPoint);
        direction.Y = 0;

        if (direction.LengthSquared() < 0.001f)
        {
            // Feed zero velocity to the agent
            Velocity = Vector3.Zero;
			_intendedDirection = Vector3.Zero;
            return;
        }

        _intendedDirection = direction.Normalized();
        Vector3 desiredVelocity = _intendedDirection * currentSpeed;

        // CRITICAL FIX: Set the Agent's velocity, NOT the MovementTarget's velocity.
        // This triggers the VelocityComputed event safely behind the scenes.
        Velocity = desiredVelocity; 
    }
    
    private void ApplyHorizontalMovement(ref Vector3 velocity, float delta)
    {
        switch (_currentState)
        {
			case State.Landed:
				velocity = Vector3.Zero;
				break;
            case State.Following:
                // _navigationVelocity is now being properly updated by the event
                velocity.X = _navigationVelocity.X;
                velocity.Z = _navigationVelocity.Z;

                RotateTowardsDirection(_intendedDirection, delta);
                break;

            case State.Idle:
                velocity.X = Mathf.MoveToward(velocity.X, 0, Deceleration * delta);
                velocity.Z = Mathf.MoveToward(velocity.Z, 0, Deceleration * delta);
                break;
        }
    }

    private void ApplyGravity(ref Vector3 velocity, float delta)
	{
		if (MovementTarget.IsOnFloor() && velocity.Y < 0)
		{
			velocity.Y = 0;
		}
		else
		{
			velocity += MovementTarget.GetGravity() * GravityScale * delta;
		}
	}

    // This is triggered by VelocityComputed
    private void UpdateNavigationVelocity(Vector3 safeVelocity)
    {
        _navigationVelocity = safeVelocity;
    }

    private void RotateTowardsDirection(Vector3 direction, float delta)
	{
		// 1. Flatten the direction so falling/jumping doesn't make the character look at the floor
		Vector3 flatDirection = new Vector3(direction.X, 0, direction.Z);

		// 2. INCREASE THE DEADZONE. Ignore microscopic physics corrections.
		if (flatDirection.LengthSquared() < 0.2f)
			return;

		// 3. Calculate target yaw
		float targetYaw = Mathf.Atan2(-flatDirection.X, -flatDirection.Z);

		// 4. Smoothly interpolate
		Vector3 rot = MovementTarget.Rotation;
		rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, 8f * delta); 
		MovementTarget.Rotation = rot;
	}

    private Vector3 GetGravityVelocity(float delta)
    {
        return MovementTarget.GetGravity() * GravityScale * delta;
    }
    #endregion
}
