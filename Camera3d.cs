using Godot;
using System;

public partial class Camera3d : Camera3D
{
	[Export] public Node3D RotationTarget;
	[Export] public float Sensitivity = 0.15f; // degrees per pixel
	[Export] public float MinPitch = -89f;
	[Export] public float MaxPitch = 89f;

	private float yawDeg = 0f;
	private float pitchDeg = 0f;

    private bool cameraEnabled = true;

	public override void _Ready()
	{
		// Initialize yaw from target if available
		if (RotationTarget != null)
		{
			// Rotation.Y is in radians; convert to degrees
			yawDeg = RotationTarget.Rotation.Y * (180f / MathF.PI);
		}
		pitchDeg = Rotation.X * (180f / MathF.PI);

		// Capture the mouse for FPS look
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mm)
		{
			if (!cameraEnabled) return;
			// Mouse motion: update yaw and pitch
			yawDeg -= mm.Relative.X * Sensitivity;
			pitchDeg -= mm.Relative.Y * Sensitivity;

			pitchDeg = Mathf.Clamp(pitchDeg, MinPitch, MaxPitch);

			// Apply yaw to the rotation target (typically the player body)
			if (RotationTarget != null)
			{
				// Convert degrees back to radians for the rotation property
				RotationTarget.Rotation = new Vector3(0f, yawDeg * (MathF.PI / 180f), 0f);
			}

			// Apply pitch to the camera (local rotation on X axis)
			Rotation = new Vector3(pitchDeg * (MathF.PI / 180f), 0f, 0f);
		}

		if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
		{
			// Toggle mouse capture with Esc
			if (keyEvent.Keycode == Key.Escape)
			{
				if (Input.MouseMode == Input.MouseModeEnum.Captured)
					Input.MouseMode = Input.MouseModeEnum.Visible;
				else
					Input.MouseMode = Input.MouseModeEnum.Captured;
                cameraEnabled = !cameraEnabled;
			}
		}
	}
}
