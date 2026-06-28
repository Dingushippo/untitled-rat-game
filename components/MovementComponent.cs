using Godot;
using System;

[GlobalClass]
public partial class MovementComponent : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export] public CharacterBody3D MovementTarget;
	[Export] public NavigationAgent3D NavigationAgent;
	[Export] public bool UseNavigationAgent = false;
	[Export] public Node3D NavigationTarget;
	[Export] public float Speed = 5.0f;
	[Export] public float JumpStrength = 5.0f;
	[Export] public float GravityScale = 1.0f;
	[Export] public float Acceleration = 20.0f;
	[Export] public float Deceleration = 16.0f;


	public Vector2 Direction { get; set; }
	public bool IsJumping { get; set; }

	private float gravity;

	public override void _Ready()
	{
		if (MovementTarget == null)
		{
			GD.PrintErr("MovementComponent requires a MovementTarget to function.");
		}
		gravity = -(float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		GD.Print("Default gravity: " + gravity);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (MovementTarget == null) return;

		Vector3 velocity = MovementTarget.Velocity;

		if (UseNavigationAgent && NavigationAgent != null && NavigationTarget != null)
		{
			velocity = HandleNavigationMovement(velocity, (float)delta);
		} 
		else
		{	
			velocity = HandleInputMovement(velocity, (float)delta);
		}

		velocity += GetGravityVelocity((float)delta);
		MovementTarget.Velocity = velocity;
		MovementTarget.MoveAndSlide();
		
	}

	private Vector3 HandleNavigationMovement(Vector3 velocity, float delta)
	{
		// Update the navigation agent's target position
		NavigationAgent.TargetPosition = NavigationTarget.GlobalPosition;

		// Get the next path point from the navigation agent
		Vector3 nextPathPoint = NavigationAgent.GetNextPathPosition();
		velocity = MovementTarget.GlobalPosition.DirectionTo(nextPathPoint) * Speed;
		if (NavigationAgent.IsNavigationFinished())
		{
			velocity = Vector3.Zero;
		}
		else
		{
			// Make movementTarget look at the next path point, smoothly rotating only around the Y axis (yaw)
			Vector3 currentRotation = MovementTarget.Rotation;
			float newYRotation = Mathf.LerpAngle(currentRotation.Y, Mathf.Atan2(-velocity.X, -velocity.Z), 0.1f);
			MovementTarget.Rotation = new Vector3(currentRotation.X, newYRotation, currentRotation.Z);
		}
		return velocity;
	}

	private Vector3 HandleInputMovement(Vector3 velocity, float delta)
	{
		// If not using navigation, apply movement based on input		
		velocity = SetMovementVelocity(velocity, (float)delta);
		velocity += GetJumpVelocity((float)delta);
		return velocity;
	}

	private Vector3 SetMovementVelocity(Vector3 currentVelocity, float delta)
	{
		// Smoothly accelerate toward the target velocity when input is present
		// and smoothly decelerate to zero when input is released. Preserve Y velocity.

		// Interpret input relative to the movement target's yaw (so forward is where the player faces)
		Vector3 inputRaw = new Vector3(Direction.X, 0, Direction.Y);
		Vector3 inputDirection = inputRaw.Length() > 0 ? inputRaw.Normalized() : Vector3.Zero;

		Vector3 target = Vector3.Zero;
		if (MovementTarget != null && inputDirection != Vector3.Zero)
		{
			// Get the yaw rotation (rotation around Y axis) from the target
			float yaw = MovementTarget.Rotation.Y;
			// Build a basis from yaw only
			Vector3 forward = new Vector3(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
			Vector3 right = new Vector3(forward.Z, 0, -forward.X); // perpendicular on XZ plane

			// Map input X to right, input Z to forward
			target = (right * inputDirection.X + forward * inputDirection.Z) * Speed;
		}
		else
		{
			target = inputDirection * Speed;
		}

		Vector3 currentXZ = new Vector3(currentVelocity.X, 0, currentVelocity.Z);

		float rate = inputDirection != Vector3.Zero ? Acceleration : Deceleration;
		float t = Mathf.Clamp(rate * delta, 0f, 1f);

		Vector3 newXZ = currentXZ.Lerp(target, t);

		return new Vector3(newXZ.X, currentVelocity.Y, newXZ.Z);
	}
	private Vector3 GetGravityVelocity(float delta)
	{
		if (MovementTarget.IsOnFloor())
		{
			return Vector3.Zero;
		}
		// GD.Print("Applying gravity: " + gravity * GravityScale);
		return MovementTarget.GetGravity() * GravityScale * delta;
	}

	private Vector3 GetJumpVelocity(float delta)
	{
		if (!IsJumping || !MovementTarget.IsOnFloor())
		{
			// GD.Print("Jumping: " + IsJumping + ", OnFloor: " + MovementTarget.IsOnFloor());
			return Vector3.Zero;
		}
		return new Vector3(0, JumpStrength, 0);
	}
}
