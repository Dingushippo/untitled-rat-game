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
}
