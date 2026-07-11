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
		// Update the navigation target if we're following something.
		if (_currentState == State.Following && NavigationTarget != null)
		{
			TargetPosition = NavigationTarget.GlobalPosition;
		}
		// Let the current state update intentions (speed, animations, etc.)
		ProcessState((float)delta);

		if (_currentState == State.Disabled) return;

		// Calculate the desired velocity.
		ApplyMovement((float)delta);
		// Actually move.
		MovementTarget.MoveAndSlide();
		// React to the result of the movement.
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
	private Vector3 _externalVelocity = Vector3.Zero;
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

		// Idle shouldn't interrupt the landing animation.
		if (isLanding)
			return;

		if (NavigationTarget != null)
			SetState(State.Following);
	}

	private void FollowingState(float delta)
	{
		if (NavigationTarget == null || IsTargetReached())
		{
			SetState(State.Idle);
			return;
		}

		TargetPosition = NavigationTarget.GlobalPosition;

		float distance = MovementTarget.GlobalPosition.DistanceTo(TargetPosition);

		if (distance <= StoppingDistance)
		{
			float t = Mathf.Clamp(distance / StoppingDistance, 0f, 1f);
			desiredSpeed = Mathf.Lerp(0f, Speed, t);
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
		// Nothing to do here yet.
		// Later you'll play the falling animation here.
	}

	private void LandedState(float delta)
	{
		if (isLanding)
			return;

		isLanding = true;

		Tween tween = CreateTween();

		tween.TweenProperty(
			MovementTarget,
			"rotation",
			Vector3.Zero,
			0.2f);

		tween.TweenCallback(Callable.From(() =>
		{
			isLanding = false;

			SetState(
				NavigationTarget != null
					? State.Following
					: State.Idle);
		}));
	}

	// private void ForceAppliedState(float delta)
	// {
	// 	desiredSpeed = 0f;

	// 	// We'll handle the actual knockback movement
	// 	// inside ApplyMovement() later.
	// }

	private void ForceAppliedState(float delta)
	{
		// Optional:
		// Play "thrown" animation here.
		// Disable AI here.
		// Disable attacks here.

		if (MovementTarget.IsOnFloor())
		{
			SetState(
				NavigationTarget != null
					? State.Following
					: State.Idle);
		}
	}

	// public void ApplyForce(Vector3 force)
	// {
	// 	_externalVelocity += force;
		
	// 	SetState(State.ForceApplied);
	// }

	public void ApplyForce(Vector3 force)
	{
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
	private void ApplyMovement(float delta)
	{
		Vector3 velocity = MovementTarget.Velocity;

		if (_currentState != State.ForceApplied)
		{
			ApplyHorizontalMovement(ref velocity, delta);
		}

		ApplyGravity(ref velocity, delta);

		MovementTarget.Velocity = velocity;
	}
	
	private void ApplyHorizontalMovement(ref Vector3 velocity, float delta)
	{
		float rate = currentSpeed < desiredSpeed
			? Acceleration
			: Deceleration;

		currentSpeed = Mathf.MoveToward(
			currentSpeed,
			desiredSpeed,
			rate * delta);

		if (_currentState != State.Following)
		{
			velocity.X = Mathf.MoveToward(
				velocity.X,
				0,
				Deceleration * delta);

			velocity.Z = Mathf.MoveToward(
				velocity.Z,
				0,
				Deceleration * delta);

			return;
		}

		Vector3 nextPoint = GetNextPathPosition();

		Vector3 direction =
			nextPoint - MovementTarget.GlobalPosition;

		direction.Y = 0;

		if (direction.LengthSquared() > 0.001f)
		{
			direction = direction.Normalized();

			// velocity.X = direction.X * currentSpeed;
			// velocity.Z = direction.Z * currentSpeed;

			Vector3 desiredVelocity = direction * desiredSpeed;
			velocity = desiredVelocity;

			RotateTowardsDirection(direction);
		}
	}
	private void ApplyGravity(ref Vector3 velocity, float delta)
	{
		if (MovementTarget.IsOnFloor())
		{
			if (velocity.Y < 0)
				velocity.Y = 0;

			return;
		}
		velocity +=
			MovementTarget.GetGravity()
			* GravityScale
			* delta;
	}
	private void UpdateNavigationVelocity(Vector3 safeVelocity)
	{
		_navigationVelocity = safeVelocity;
	}

	// private void HandleNavigationMovement(float delta)
	// {
	// 	// Update the navigation agent's target position
	// 	TargetPosition = NavigationTarget.GlobalPosition;

	// 	float distanceToTarget = MovementTarget.GlobalPosition.DistanceTo(TargetPosition);
	// 	float targetSpeed = Speed;

	// 	if (distanceToTarget <= StoppingDistance)
	// 	{
	// 		float brakingProgress = Mathf.Clamp(distanceToTarget / StoppingDistance, 0f, 1f);
	// 		targetSpeed = Mathf.Lerp(0f, Speed, brakingProgress);
	// 	}

	// 	float rate = currentSpeed < targetSpeed ? Acceleration : Deceleration;
	// 	currentSpeed = Mathf.MoveToward(currentSpeed, targetSpeed, rate * delta);

	// 	// Get the next path point from the navigation agent
	// 	Vector3 nextPathPoint = GetNextPathPosition();
	// 	Vector3 newDirection = MovementTarget.GlobalPosition.DirectionTo(nextPathPoint);
	// 	newDirection.Y = 0.0f;
	// 	newDirection = newDirection.Normalized();

	// 	Vector3 newVelocity = newDirection * currentSpeed + GetGravityVelocity(delta);

	// 	SetMovementTargetVelocity(newVelocity);

	// 	if (IsNavigationFinished())
	// 	{
	// 		SetMovementTargetVelocity(Vector3.Zero);
	// 		RotateTowardsPosition(NavigationTarget.GlobalPosition);
	// 	}
	// 	else
	// 	{
	// 		RotateTowardsDirection(newVelocity);
	// 	}
	// }

	private void RotateTowardsPosition(Vector3 worldPosition)
	{
		Vector3 direction = (worldPosition - MovementTarget.GlobalPosition).Normalized();
		RotateTowardsDirection(direction);
	}

	private void RotateTowardsDirection(Vector3 direction)
	{
		if (direction.LengthSquared() < 0.001f)
			return;

		float targetYaw = Mathf.Atan2(-direction.X, -direction.Z);

		Vector3 rot = MovementTarget.Rotation;
		rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, 0.1f);
		MovementTarget.Rotation = rot;
	}

	private Vector3 GetGravityVelocity(float delta)
	{
		return MovementTarget.GetGravity() * GravityScale * delta;
	}
	#endregion
}
