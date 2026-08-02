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

    [ExportGroup("Throw Charge")]
    [Export] public float ChargePitchDegrees = 1.2f;
    [Export] public float ChargePullDistance = 0.05f;
    [Export] public float ChargeFovZoom = 6f;
    [Export] public float ChargeReturnDuration = 0.18f;

    [ExportGroup("Throw Impact")]
    [Export] public float ImpactPitchDegrees = 2.2f;
    [Export] public float ImpactRollDegrees = 1.4f;
    [Export] public float ImpactPunchDistance = 0.07f;
    [Export] public float ImpactFovPunch = 7f;
    [Export] public float MinImpactScale = 0.3f;
    [Export(PropertyHint.Range, "0,0.5")] public float ImpactAttackRatio = 0.18f;

    public Node3D LookingAtObject;
    public Vector3? LookingAtCollisionPosition;
    public float YOffset = 0f;

    private float _yawDeg = 0f;
    private float _pitchRad = 0f;
    private bool _cameraEnabled = true;
    private float _originalFov;

    private Vector3 _basePosition;
    private Vector3 _baseRotation;

    // Additive offsets driven by tweens, applied on top of the look rotation.
    private float _kickPitch;
    private float _kickRoll;
    private float _kickZ;
    private float _fovOffset;

    private Tween _cameraTween;

    private readonly record struct CamPose(float Pitch, float Roll, float Z, float Fov);

    private static readonly CamPose RestPose = new(0f, 0f, 0f, 0f);
    private CamPose CurrentPose => new(_kickPitch, _kickRoll, _kickZ, _fovOffset);

    public override void _Ready()
    {
        // Initialize yaw from target if available
        if (RotationTarget != null)
        {
            // Rotation.Y is in radians; convert to degrees
            _yawDeg = RotationTarget.Rotation.Y * (180f / MathF.PI);
        }
        _pitchRad = Rotation.X;
        _originalFov = Fov;
        _basePosition = Position;
        _baseRotation = Rotation;

        // Capture the mouse for FPS look
        Input.MouseMode = Input.MouseModeEnum.Captured;

        EventBus.Subscribe(Event.CameraImpact, OnCameraImpact);
        EventBus.Subscribe(Event.CameraCharge, OnCameraCharge);
        EventBus.Subscribe(Event.CameraChargeReset, OnCameraChargeReset);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.CameraImpact, OnCameraImpact);
        EventBus.Unsubscribe(Event.CameraCharge, OnCameraCharge);
        EventBus.Unsubscribe(Event.CameraChargeReset, OnCameraChargeReset);
    }

    public override void _Process(double delta)
    {
        ApplyPose();
    }

    private void ApplyPose()
    {
        Vector3 rotation = _baseRotation;
        rotation.X = _pitchRad + _kickPitch;
        rotation.Z = _baseRotation.Z + _kickRoll;
        Rotation = rotation;

        Position = _basePosition + new Vector3(0f, YOffset, _kickZ);
        Fov = _originalFov + _fovOffset;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mm)
        {
            if (!_cameraEnabled) return;

            RotationTarget.RotateY(-mm.Relative.X * Sensitivity * 0.01f);

            _pitchRad = Mathf.Clamp(
                _pitchRad - mm.Relative.Y * Sensitivity * 0.01f,
                Mathf.DegToRad(-80),
                Mathf.DegToRad(80)
            );
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

    private void OnCameraCharge(params object[] args)
    {
        float duration = (float)args[0];
        float delay = args.Length > 1 ? (float)args[1] : 0f;

        // Wind-up: pull back, tilt down and narrow the FOV to build tension.
        CamPose target = new(
            -Mathf.DegToRad(ChargePitchDegrees),
            0f,
            ChargePullDistance,
            -ChargeFovZoom
        );

        _cameraTween?.Kill();
        _cameraTween = CreateTween();
        _cameraTween.SetParallel(true);
        _cameraTween.SetEase(Tween.EaseType.InOut);
        _cameraTween.SetTrans(Tween.TransitionType.Sine);
        TweenPose(CurrentPose, target, duration, delay);
    }

    private void OnCameraChargeReset(params object[] args)
    {
        _cameraTween?.Kill();
        _cameraTween = CreateTween();
        _cameraTween.SetParallel(true);
        _cameraTween.SetEase(Tween.EaseType.Out);
        _cameraTween.SetTrans(Tween.TransitionType.Sine);
        TweenPose(CurrentPose, RestPose, ChargeReturnDuration);
    }

    private void OnCameraImpact(params object[] args)
    {
        float charge = args.Length > 0 ? (float)args[0] : 1f;
        float duration = args.Length > 1 ? (float)args[1] : 0.35f;
        PlayImpact(charge, duration);
    }

    private void PlayImpact(float charge, float duration)
    {
        GD.Print($"CameraImpact! - args: {charge}, {duration}");
        float scale = Mathf.Lerp(MinImpactScale, 1f, Mathf.Clamp(charge, 0f, 1f));

        // Release: snap forward and up, roll slightly, widen the FOV.
        CamPose peak = new(
            Mathf.DegToRad(ImpactPitchDegrees) * scale,
            -Mathf.DegToRad(ImpactRollDegrees) * scale,
            -ImpactPunchDistance * scale,
            ImpactFovPunch * scale
        );

        float attack = duration * ImpactAttackRatio;
        float recover = Mathf.Max(duration - attack, 0.01f);

        _cameraTween?.Kill();
        _cameraTween = CreateTween();
        _cameraTween.SetParallel(true);

        _cameraTween.SetEase(Tween.EaseType.Out);
        _cameraTween.SetTrans(Tween.TransitionType.Expo);
        TweenPose(CurrentPose, peak, attack);

        _cameraTween.Chain();
        _cameraTween.SetEase(Tween.EaseType.Out);
        _cameraTween.SetTrans(Tween.TransitionType.Back);
        TweenPose(peak, RestPose, recover);
    }

    private void TweenPose(CamPose from, CamPose to, float duration, float delay = 0f)
    {
        TweenChannel(v => _kickPitch = v, from.Pitch, to.Pitch, duration, delay);
        TweenChannel(v => _kickRoll = v, from.Roll, to.Roll, duration, delay);
        TweenChannel(v => _kickZ = v, from.Z, to.Z, duration, delay);
        TweenChannel(v => _fovOffset = v, from.Fov, to.Fov, duration, delay);
    }

    private void TweenChannel(Action<float> setter, float from, float to, float duration, float delay)
    {
        MethodTweener tweener = _cameraTween.TweenMethod(Callable.From<float>(setter), from, to, duration);
        if (delay > 0f)
        {
            tweener.SetDelay(delay);
        }
    }
}
