using Godot;
using Godot.Collections;
using System;

public partial class PlayerCamera : Camera3D
{
	[Export] public Node3D RotationTarget;
	[Export] public bool DebugAimMarker = false;
	[Export] public float Sensitivity = 0.15f; // degrees per pixel
	[Export] public float MinPitch = -89f;
	[Export] public float MaxPitch = 89f;

	public Node3D lookingAtObject;
	public Vector3? lookingAtCollisionPosition;

	private float _yawDeg = 0f;
	private float _pitchDeg = 0f;

    private bool _cameraEnabled = true;
	private Node3D _aimMarker;

	

	public override void _Ready()
	{
		// Initialize yaw from target if available
		if (RotationTarget != null)
		{
			// Rotation.Y is in radians; convert to degrees
			_yawDeg = RotationTarget.Rotation.Y * (180f / MathF.PI);
		}
		_pitchDeg = Rotation.X * (180f / MathF.PI);

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

		_aimMarker = new Node3D();
		_aimMarker.Name = "AimMarker_Debug";
		_aimMarker.AddChild(meshInstance);

		AddChild(_aimMarker);
	}

	private void RemoveDebugAimMarker()
	{
		if (_aimMarker != null)
		{
			_aimMarker.QueueFree();
			_aimMarker = null;
		}
	}

    public override void _PhysicsProcess(double delta)
    {
		// Raycast from the camera forward to find the floor and position AimMarker there (if assigned)
		if (_aimMarker == null)
			return;

		Vector3 from = GlobalTransform.Origin;
		Vector3 to = from + -GlobalTransform.Basis.Z * 1000f;

		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = new PhysicsRayQueryParameters3D
        {
            From = from,
            To = to
        };
        Dictionary result = spaceState.IntersectRay(query);
		if (result != null && result.Count > 0)
		{
			// if ((Vector3)result["position"] == _lookingAtCollisionPosition) return;
			if (DebugAimMarker) _aimMarker.Visible = true;

			lookingAtObject = (Node3D)result["collider"];
			lookingAtCollisionPosition = (Vector3)result["position"];
			_aimMarker.GlobalPosition = (Vector3)lookingAtCollisionPosition;
		}
		else if (_aimMarker.Visible)	_aimMarker.Visible = false;
		else
		{
			lookingAtObject = null;
			lookingAtCollisionPosition = null;
		}
    }



	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mm)
		{
			if (!_cameraEnabled) return;
			
			RotationTarget.RotateY(-mm.Relative.X * Sensitivity * 0.01f);
			RotateX(-mm.Relative.Y * Sensitivity * 0.01f);

			Vector3 cameraRot = Rotation;
			cameraRot.X = Mathf.Clamp(cameraRot.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
			Rotation = cameraRot;
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
                _cameraEnabled = !_cameraEnabled;
			}
		}
	}

	private void OnMouseClick(MouseClickEvent evt)
	{
		if (evt.ButtonIndex != (int)MouseButton.Left) return;
		
		GD.Print($"Mouse clicked: Button {evt.ButtonIndex} at {_aimMarker.GlobalPosition}");
		EventBus.Publish(new DebugAimMarkerEvent(_aimMarker.GlobalPosition));
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
