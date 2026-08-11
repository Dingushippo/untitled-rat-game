using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

public partial class PlayerCamera : Camera3D
{
    [Export] public Player Player;
    [Export] public bool DebugAimMarker = false;
    [Export] public PlayerCameraTuning Tuning;
    [Export] public Node3D HeadNode;
    [Export] public Node3D HandNode;
    [Export] public Noise ShakeNoise;

    public float YOffset = 0f;

    private float _yawDeg = 0f;
    private float _pitchRad = 0f;
    private bool _cameraEnabled = true;
    private float _originalFov;

    private Vector3 _basePosition;
    private Vector3 _baseRotation;
    private Vector3 _handOffset;

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
        if (Player != null)
        {
            // Rotation.Y is in radians; convert to degrees
            _yawDeg = Player.Rotation.Y * (180f / MathF.PI);
        }
        _pitchRad = Rotation.X;
        _originalFov = Fov;
        _basePosition = Position;
        _baseRotation = Rotation;
        _handOffset = HandNode.Position;

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
        HandleHeadbob((float)delta);
        HandleFovMovementChange((float)delta);
        ApplyPose((float)delta);
    }

    private void ApplyPose(float delta)
    {
        Vector3 pitch = _baseRotation;
        Vector3 yaw = _baseRotation;
        pitch.X = _pitchRad + _kickPitch;
        yaw.Z = _baseRotation.Z + _kickRoll;
        Rotation = yaw;
        HeadNode.Rotation = pitch;
        Position = _basePosition + new Vector3(0f, YOffset, _kickZ) + _bobOffset;
        HandNode.Position = _handOffset + new Vector3(0f, YOffset, _kickZ);
        Fov = _originalFov + _fovOffset;
    }

    private float _bobTime = 0f;
    private Vector3 _bobOffset;
    private float _bobSpeed;
    private float _bobStrength;
    private float _blendSpeed = 10f;

    private void HandleHeadbob(float delta)
    {
        float blendDelta = delta * _blendSpeed;
        if (Player.Velocity.IsZeroApprox() || Player.CrouchComponent.IsCrouching || !Player.IsOnFloor())
        {
            _bobTime = 0;
            _bobOffset = _bobOffset.Lerp(Vector3.Zero, blendDelta);
            return;
        }
        _bobSpeed = Mathf.Remap(
            Player.CurrentSpeed,
            Player.Speed,
            Player.SprintSpeed,
            Tuning.BobSpeed,
            Tuning.BobSpeedSprint
        );
        _bobStrength = Mathf.Remap(
            Player.CurrentSpeed,
            Player.Speed,
            Player.SprintSpeed,
            Tuning.BobStrength,
            Tuning.BobStrengthSprint
        );

        _bobTime += delta;
        _bobOffset = _bobOffset.Lerp(new(
            Mathf.Sin(_bobSpeed * _bobTime / 2) * _bobStrength,
            Mathf.Cos(_bobSpeed * _bobTime) * _bobStrength / 2,
            0f
        ), blendDelta);
    }

    private void HandleFovMovementChange(float delta)
    {
        float blendDelta = delta * _blendSpeed;
        if (Player.Velocity.IsZeroApprox())
        {
            _fovOffset = Mathf.Lerp(_fovOffset, 0, blendDelta);
            return;
        }


        float newOffset;

        if (Player.CrouchComponent.IsCrouching)
            newOffset = Tuning.SlideFovOffset;
        else
            newOffset = Mathf.Remap(
                Player.CurrentSpeed,
                Player.Speed,
                Player.SprintSpeed,
                Tuning.WalkFovOffset,
                Tuning.RunFovOffset
            );
        _fovOffset = Mathf.Lerp(_fovOffset, newOffset, blendDelta);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mm)
        {
            if (!_cameraEnabled) return;

            Player.RotateY(-mm.Relative.X * Tuning.Sensitivity * 0.01f);

            _pitchRad = Mathf.Clamp(
                _pitchRad - mm.Relative.Y * Tuning.Sensitivity * 0.01f,
                Mathf.DegToRad(-80),
                Mathf.DegToRad(80)
            );
        }

        if (@event is InputEventKey key && key.Pressed)
        {
            if (key.Keycode == Key.Key1)
            {
                _cameraTween?.Kill();
                _cameraTween = CreateTween();
                _cameraTween.SetParallel();
                CamPose newPose = new(Tuning.Pitch, Tuning.Roll, Tuning.Z, Tuning.Fov);
                TweenPose(CurrentPose, newPose, 0.2f);
            }
            if (key.Keycode == Key.Key2)
            {
                _cameraTween?.Kill();
                _cameraTween = CreateTween();
                _cameraTween.SetParallel();
                TweenPose(CurrentPose, RestPose, 0.2f);
            }
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

    public void SetCameraInputEnabled(bool enabled)
    {
        _cameraEnabled = enabled;
    }

    private void OnCameraCharge(params object[] args)
    {
        float duration = (float)args[0];
        float delay = args.Length > 1 ? (float)args[1] : 0f;

        // Wind-up: pull back, tilt down and narrow the FOV to build tension.
        CamPose target = new(
            -Mathf.DegToRad(Tuning.ChargePitchDegrees),
            0f,
            Tuning.ChargePullDistance,
            -Tuning.ChargeFovZoom
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
        TweenPose(CurrentPose, RestPose, Tuning.ChargeReturnDuration);
    }

    private void OnCameraImpact(params object[] args)
    {
        float charge = args.Length > 0 ? (float)args[0] : 1f;
        float duration = args.Length > 1 ? (float)args[1] : 0.35f;
        PlayImpact(charge, duration);
    }

    private void PlayImpact(float charge, float duration)
    {
        float scale = Mathf.Lerp(Tuning.MinImpactScale, 1f, Mathf.Clamp(charge, 0f, 1f));

        // Release: snap forward and up, roll slightly, widen the FOV.
        CamPose peak = new(
            Mathf.DegToRad(Tuning.ImpactPitchDegrees) * scale,
            -Mathf.DegToRad(Tuning.ImpactRollDegrees) * scale,
            -Tuning.ImpactPunchDistance * scale,
            Tuning.ImpactFovPunch * scale
        );

        float attack = duration * Tuning.ImpactAttackRatio;
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
