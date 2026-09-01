using System;
using Godot;

public partial class PlayerCamera : Camera3D
{
    [Export]
    public Player Player;

    [Export]
    public bool DebugAimMarker = false;

    [Export]
    public PlayerCameraTuning Tuning;

    [Export]
    public Node3D HeadNode;

    [Export]
    public Node3D HandNode;

    [Export]
    public Noise ShakeNoise;

    public float YOffset = 0f;
    private float _yawRad;
    private float _pitchRad = 0f;
    private bool _cameraEnabled = true;

    private float _originalFov;
    private Vector3 _basePosition;
    private Vector3 _baseRotation;
    private Vector3 _handOffset;

    // Additive offsets driven by tweens, applied on top of the look rotation.
    private float _pitchOffset;
    private float _pitchOffsetTarget;
    private float _rollOffset;
    private float _rollOffsetTarget;
    private float _kickZ;
    private float _kickZTarget;
    private float _fovOffset;
    private float _fovOffsetTarget;
    private float _xOffset;
    private float _xOffsetTarget;

    // Headbob specific
    private float _bobTime = 0f;
    private Vector3 _bobOffset;
    private float _bobSpeed;
    private float _bobStrength;
    private float _blendSpeed = 10f;

    // private Tween _cameraTween;

    private readonly record struct CamPose(float Pitch, float Roll, float Z, float Fov, float Side);

    private static readonly CamPose _restPose = new(0f, 0f, 0f, 0f, 0f);
    private CamPose CurrentPose => new(_pitchOffset, _rollOffset, _kickZ, _fovOffset, _xOffset);

    public Vector3 ForwardDirection => -GlobalBasis.Z;

    public override void _Ready()
    {
        // Initialize yaw from target if available
        _pitchRad = HeadNode.Rotation.X;
        _yawRad = Player.Rotation.Y;
        _originalFov = Fov;
        _basePosition = Position;
        _baseRotation = HeadNode.Rotation;
        _handOffset = HandNode.Position;

        // Capture the mouse for FPS look
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _ExitTree()
    {
    }

    public override void _Process(double delta)
    {
        BlendOffsets((float)delta);
        HandleHeadbob((float)delta);
        HandleFovMovementChange((float)delta);
        ApplyPose((float)delta);
    }

    private float SmoothBlend(float from, float to, float blend)
    {
        return Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1, blend));
    }
    private void BlendOffsets(float delta)
    {
        float blend = _blendSpeed * delta;
        _pitchOffset = SmoothBlend(_pitchOffset, _pitchOffsetTarget, blend);
        _rollOffset = SmoothBlend(_rollOffset, _rollOffsetTarget, blend);
        _kickZ = SmoothBlend(_kickZ, _kickZTarget, blend);
        _fovOffset = SmoothBlend(_fovOffset, _fovOffsetTarget, blend);
        _xOffset = SmoothBlend(_xOffset, _xOffsetTarget, blend);
    }

    private void ApplyPose(float delta)
    {
        // Set player rotation
        Vector3 playerRotation = _baseRotation;
        playerRotation.Y = _yawRad;
        Player.Rotation = playerRotation;

        // Set head pitch
        Vector3 rotation = _baseRotation;
        rotation.X = _pitchRad + _pitchOffset;
        HeadNode.Rotation = rotation;

        // Set additional offsets
        Position = _basePosition + new Vector3(_xOffset, YOffset, _kickZ) + _bobOffset;
        HandNode.Position = _handOffset + new Vector3(0f, YOffset, _kickZ);
        Fov = _originalFov + _fovOffset;
    }



    private void HandleHeadbob(float delta)
    {
        float blendDelta = delta * _blendSpeed;

        _bobTime += delta;
        _bobOffset = _bobOffset.Lerp(
            new(
                Mathf.Sin(_bobSpeed * _bobTime / 2) * _bobStrength,
                Mathf.Cos(_bobSpeed * _bobTime) * _bobStrength / 2,
                0f
            ),
            blendDelta
        );
    }

    public void SetBobVariables(float speed, float strength)
    {
        _bobSpeed = speed;
        _bobStrength = strength;
    }

    public void SetDesiredFovOffset(float offset)
    {

    }

    private void HandleFovMovementChange(float delta)
    {
        float blendDelta = delta * _blendSpeed;
        // if (Player.Velocity.IsZeroApprox())
        // {
        //     _fovOffset = Mathf.Lerp(_fovOffset, 0, blendDelta);
        //     return;
        // }

        // float newOffset;

        // if (Player.IsMovementState<PlayerSlideState>())
        //     newOffset = Tuning.SlideFovOffset;
        // else
        //     newOffset = Mathf.Remap(
        //         Player.Velocity.Length(),
        //         Player.Tuning.Speed,
        //         Player.Tuning.SprintSpeed,
        //         Tuning.WalkFovOffset,
        //         Tuning.RunFovOffset
        //     );
        // _fovOffset = Mathf.Lerp(_fovOffset, newOffset, blendDelta);
    }



    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mm)
        {
            if (!_cameraEnabled)
                return;
            _yawRad += -mm.Relative.X * Tuning.Sensitivity * 0.01f;
            _pitchRad = Mathf.Clamp(
                _pitchRad - mm.Relative.Y * Tuning.Sensitivity * 0.01f,
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

    public void SetCameraInputEnabled(bool enabled)
    {
        _cameraEnabled = enabled;
    }
}
