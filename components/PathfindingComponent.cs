using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class PathfindingComponent : NavigationAgent3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public CharacterBody3D MovementTarget;
	[Export] public Node3D NavigationTarget;
	[Export] public float Speed = 5.0f;
	[Export] public float JumpStrength = 5.0f;
	[Export] public float GravityScale = 1.0f;
	[Export] public float Acceleration = 20.0f;
	[Export] public float Deceleration = 16.0f;

	[Export] public float StoppingDistance = 0.5f;

	public Vector2 Direction { get; set; }
	public bool IsJumping { get; set; }

	private float gravity;
	private float currentSpeed = 0f;

	public override void _Ready()
	{
		if (MovementTarget == null)
		{
			GD.PrintErr("PathfindingComponent requires a MovementTarget to function.");
		}

		gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		VelocityComputed += SetMovementTargetVelocity;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (NavigationTarget == null) return;

		HandleNavigationMovement((float)delta);
		MovementTarget.MoveAndSlide();

	}

	private void SetMovementTargetVelocity(Vector3 velocity)
	{
		MovementTarget.Velocity = velocity;
	}

	private void HandleNavigationMovement(float delta)
	{
		// Update the navigation agent's target position
		TargetPosition = NavigationTarget.GlobalPosition;

		float distanceToTarget = MovementTarget.GlobalPosition.DistanceTo(TargetPosition);
		float targetSpeed = Speed;

		if (distanceToTarget <= StoppingDistance)
		{
			float brakingProgress = Mathf.Clamp(distanceToTarget / StoppingDistance, 0f, 1f);
			targetSpeed = Mathf.Lerp(0f, Speed, brakingProgress);
		}

		float rate = currentSpeed < targetSpeed ? Acceleration : Deceleration;
		currentSpeed = Mathf.MoveToward(currentSpeed, targetSpeed, rate * delta);

		// Get the next path point from the navigation agent
		Vector3 nextPathPoint = GetNextPathPosition();
		Vector3 newVelocity = (
			MovementTarget.GlobalPosition.DirectionTo(nextPathPoint) * currentSpeed
			+ GetGravityVelocity(delta)
		);

		SetMovementTargetVelocity(newVelocity);

		if (IsNavigationFinished())
		{
			SetMovementTargetVelocity(Vector3.Zero);
			RotateTowardsPosition(NavigationTarget.GlobalPosition);
		}
		else
		{
			RotateTowardsDirection(newVelocity);
		}
	}

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
	if (MovementTarget.IsOnFloor())
	{
		return Vector3.Zero;
	}
	return Vector3.Down * gravity * GravityScale * delta;
}
}
