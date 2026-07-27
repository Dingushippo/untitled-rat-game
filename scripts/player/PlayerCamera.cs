using Godot;
using Godot.Collections;
using System;
using System.Runtime.Serialization;

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
    private float _originalFov;




    public override void _Ready()
    {
        // Initialize yaw from target if available
        if (RotationTarget != null)
        {
            // Rotation.Y is in radians; convert to degrees
            _yawDeg = RotationTarget.Rotation.Y * (180f / MathF.PI);
        }
        _pitchDeg = Rotation.X * (180f / MathF.PI);
        _originalFov = Fov;

        // Capture the mouse for FPS look
        Input.MouseMode = Input.MouseModeEnum.Captured;

        EventBus.Subscribe(Event.CameraImpact, OnCameraImpact);
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

        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Key1)
        {
            OnCameraImpact(0.15f, 0.35f);
        }
    }

    private void OnCameraImpact(params object[] args)
    {
        float force = (float)args[0];
        float duration = (float)args[1];

        Tween impactTween = CreateTween();
        impactTween.SetParallel(true);
        impactTween.SetEase(Tween.EaseType.Out);
        impactTween.SetTrans(Tween.TransitionType.Cubic);
        impactTween.TweenProperty(this, "position:z", force, duration * 0.25f);
        impactTween.TweenProperty(this, "fov", _originalFov + force * _originalFov, duration * 0.25f);
        impactTween.Chain();
        impactTween.SetEase(Tween.EaseType.Out);
        impactTween.SetTrans(Tween.TransitionType.Quart);
        impactTween.TweenProperty(this, "position:z", 0, duration * 0.75f);
        impactTween.TweenProperty(this, "fov", _originalFov, duration * 0.75f);
    }
}
