using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class ThrowComponent : Node3D
{
    [Export] public Player Player;
    [Export] public ThrowPreview ThrowPreview;
    [Export] public Node3D HandNode;
    [Export] public ThrowType ThrowType;
    [Export] public ThrowTuning Tuning;

    private float _currentForce = 0;
    private Vector3 _gravity;
    private ThrowPath _currentPath;
    private bool _preview = false;
    private bool _isCharging = false;
    private float _aimYaw = 0f;

    public override void _Ready()
    {
        Tuning ??= new ThrowTuning();

        _gravity = Player.GetGravity();
        _currentForce = Tuning.ThrowForce;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_preview)
        {
            using (Profiler.Sample("throw.simulate"))
            {
                _currentPath = ThrowType.Simulate(BuildContext(Player.GrabComponent.CurrentGrabbed));
                if (!_currentPath.Homing && _currentPath.ThrowTarget.IsValid && _currentPath.Points.Length >= 2)
                {
                    Vector3 right = Player.Camera.GlobalBasis.X;
                    Vector3 toEnd = _currentPath.End - Player.Camera.GlobalPosition;
                    float lateral = right.Dot(toEnd);
                    float forward = Mathf.Max(toEnd.Dot(-Player.Camera.GlobalBasis.Z), Tuning.MinAimDistance);
                    _aimYaw = Mathf.Clamp(
                        _aimYaw - Tuning.AimGain * Mathf.Atan2(lateral, forward),
                        -Mathf.DegToRad(Tuning.MaxAimCorrectionDegrees),
                        -Mathf.DegToRad(Tuning.MaxAimCorrectionDegrees)
                    );
                    _currentPath = ThrowType.Simulate(BuildContext(Player.GrabComponent.CurrentGrabbed));
                }
            }
            ThrowPreview.ShowPreview(_currentPath);
        }
        else
        {
            ThrowPreview.HidePreview();
        }
    }

    /// <summary>Aim state for a simulation run. Public so the profiler bench can drive it headlessly.</summary>
    public ThrowContext BuildContext(Rat rat) => new(
        rat,
        GlobalPosition,
        (-Player.Camera.GlobalBasis.Z).Rotated(Vector3.Right, Mathf.DegToRad(Tuning.AngleAdjust)).Rotated(Vector3.Up, _aimYaw).Normalized(),
        _currentForce / rat.RatDef.Mass,
        _gravity,
        Tuning.AscentGravityScale,
        Tuning.DescentGravityScale,
        Tuning.DescentBlendSpeed,
        Tuning.Step,
        Tuning.MaxPoints
    );


    private Tween _chargeTween;
    public void StartDelayedCharge()
    {
        _chargeTween = CreateTween();
        _chargeTween.TweenMethod(
            Callable.From<float>(v => _currentForce = v),
            Tuning.ThrowForce,
            Tuning.MaxThrowForce,
            Tuning.ChargeDuration
        ).SetDelay(Tuning.ChargeStartDelay);
        _isCharging = true;

        EventBus.Publish(Event.CameraCharge, Tuning.ChargeDuration, Tuning.ChargeStartDelay);
    }

    public void ResetCharge()
    {
        if (_chargeTween != null)
        {
            _chargeTween.Kill();
        }
        _currentForce = Tuning.ThrowForce;

        // Only when the charge is abandoned - a completed throw hands the
        // camera over to the impact effect instead.
        if (_isCharging)
        {
            _isCharging = false;
            EventBus.Publish(Event.CameraChargeReset);
        }
    }

    public void Throw(Rat rat)
    {
        float chargeAmount = Mathf.IsEqualApprox(Tuning.MaxThrowForce, Tuning.ThrowForce)
            ? 1f
            : Mathf.Clamp((_currentForce - Tuning.ThrowForce) / (Tuning.MaxThrowForce - Tuning.ThrowForce), 0f, 1f);
        // Flight speed comes from the simulated path itself, so charge only has to shape the arc.        

        if (_currentPath.ThrowTarget.IsSlot)
        {
            _currentPath.ThrowTarget.WorkSlot.TryReserve(rat);
        }

        rat.GetState<RatCurveState>().Configure(_currentPath);
        rat.ChangeState<RatCurveState>();

        _isCharging = false;
        ResetCharge();

        EventBus.Publish(new RatThrown());
        EventBus.Publish(Event.CameraImpact, chargeAmount, 0.35f);
    }

    public void Enable()
    {
        _preview = true;
        _aimYaw = 0f;
    }

    public void Reset()
    {
        _preview = false;
        _aimYaw = 0f;
        ResetCharge();
    }
}