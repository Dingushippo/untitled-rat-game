using Godot;
using System;

public partial class MovementComponent : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export] public CharacterBody3D MovementTarget;
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

		velocity = SetMovementVelocity(velocity, (float)delta);
		velocity += GetJumpVelocity((float)delta);
		velocity += GetGravityVelocity((float)delta);
		
		GD.Print("Velocity: " + velocity);

		MovementTarget.Velocity = velocity;
		MovementTarget.MoveAndSlide();
	}

	private Vector3 SetMovementVelocity(Vector3 currentVelocity, float delta)
	{
		// Smoothly accelerate toward the target velocity when input is present
		// and smoothly decelerate to zero when input is released. Preserve Y velocity.

		Vector3 inputRaw = new Vector3(Direction.X, 0, Direction.Y);
		Vector3 inputDirection = inputRaw.Length() > 0 ? inputRaw.Normalized() : Vector3.Zero;

		Vector3 target = inputDirection * Speed;

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
