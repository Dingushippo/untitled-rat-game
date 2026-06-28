using Godot;
using System;

public partial class Camera3d : Camera3D
{
	[Export] public Node3D RotationTarget;
	[Export] public bool DebugAimMarker = false;
	[Export] public float Sensitivity = 0.15f; // degrees per pixel
	[Export] public float MinPitch = -89f;
	[Export] public float MaxPitch = 89f;

	private float yawDeg = 0f;
	private float pitchDeg = 0f;

    private bool cameraEnabled = true;
	private Node3D AimMarker;

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

		// Auto-create an AimMarker when debug is enabled
		if (DebugAimMarker)
		{
			CreateDebugAimMarker();
		}

		EventBus.Subscribe<MouseClickEvent>(OnMouseClick);
	}

	private void CreateDebugAimMarker()
	{
		// Create a simple MeshInstance3D with a SphereMesh as the visual marker
		var meshInstance = new MeshInstance3D();
		var sphere = new SphereMesh();
		sphere.Radius = 0.15f / 2;
		sphere.Height = 0.15f;
		meshInstance.Mesh = sphere;
		// Make it slightly emissive or colored so it's visible
		var mat = new StandardMaterial3D();
		mat.AlbedoColor = new Color(1f, 0f, 0f);
		meshInstance.SetSurfaceOverrideMaterial(0, mat);

		AimMarker = new Node3D();
		AimMarker.Name = "AimMarker_Debug";
		AimMarker.AddChild(meshInstance);

		AddChild(AimMarker);
	}

	private void RemoveDebugAimMarker()
	{
		if (AimMarker != null)
		{
			AimMarker.QueueFree();
			AimMarker = null;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		// Raycast from the camera forward to find the floor and position AimMarker there (if assigned)
		if (AimMarker == null)
			return;

		Vector3 from = GlobalTransform.Origin;
		// In Godot, forward is -Z
		Vector3 to = from + -GlobalTransform.Basis.Z * 1000f;

		var spaceState = GetWorld3D().DirectSpaceState;
		// Use the correct overload for IntersectRay
		var query = new PhysicsRayQueryParameters3D();
		query.From = from;
		query.To = to;
		var result = spaceState.IntersectRay(query);
		if (result != null && result.Count > 0 && result.ContainsKey("position"))
		{
			if (!AimMarker.Visible) AimMarker.Visible = true;
			
			Vector3 hitPos = (Vector3)result["position"];
			Vector3 hitNormal = result.ContainsKey("normal") ? (Vector3)result["normal"] : Vector3.Up;
			// Move the marker slightly above the surface to avoid z-fighting
			AimMarker.GlobalPosition = hitPos + hitNormal * 0.01f;
			// Orient the marker to match the surface normal
			// AimMarker.LookAt(hitPos + hitNormal, Vector3.Up);
		}
		else if (AimMarker.Visible)	AimMarker.Visible = false;
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

	private void OnMouseClick(MouseClickEvent evt)
	{
		GD.Print($"Mouse clicked: Button {evt.ButtonIndex} at {AimMarker.GlobalPosition}");
		EventBus.Publish(new DebugAimMarkerEvent(AimMarker.GlobalPosition));
	}
}

public class DebugAimMarkerEvent
{
	public Vector3 MarkerPosition { get; }

	public DebugAimMarkerEvent(Vector3 markerPosition)
	{
		MarkerPosition = markerPosition;
	}

}
