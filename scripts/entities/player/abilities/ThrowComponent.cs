using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class ThrowComponent : Node3D
{
    [Export] public Player Player;
    [Export] public Mesh ReticleMesh;
    [Export] public ThrowType ThrowType;
    [Export] public ThrowTuning Tuning;

    [ExportGroup("Path colours")]
    [Export] public Color FreeThrowColor = Colors.Red;
    [Export] public Color SlotThrowColor = Colors.Green;
    [Export] public Color IntakeThrowColor = Colors.DeepSkyBlue;

    private MeshInstance3D _pathMeshInstance;
    private MeshInstance3D _reticleMeshInstance;
    private ImmediateMesh _immediateMesh;
    private OrmMaterial3D _material;
    private float _currentForce = 0;
    private Vector3 _gravity;
    private ThrowPath _currentPath;
    private bool _preview = false;
    private bool _isCharging = false;
    public override void _Ready()
    {
        Tuning ??= new ThrowTuning();

        _gravity = Player.GetGravity();
        _currentForce = Tuning.ThrowForce;

        _pathMeshInstance = new();
        AddChild(_pathMeshInstance);

        _reticleMeshInstance = new();
        AddChild(_reticleMeshInstance);
        _reticleMeshInstance.Mesh = ReticleMesh;
        _reticleMeshInstance.Hide();

        // Init mesh
        _immediateMesh = new ImmediateMesh();
        _pathMeshInstance.Mesh = _immediateMesh;

        _material = new OrmMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            AlbedoColor = Colors.Red

        };
    }


    public override void _PhysicsProcess(double delta)
    {
        if (_preview)
        {
            using (Profiler.Sample("throw.simulate"))
            {
                _currentPath = ThrowType.Simulate(BuildContext(Player.GrabComponent.CurrentGrabbed));
            }

            _material.AlbedoColor = PathColor(_currentPath);

            using (Profiler.Sample("throw.mesh"))
            {
                GenerateMesh();
            }

            SetReticle();
        }
    }

    /// <summary>Aim state for a simulation run. Public so the profiler bench can drive it headlessly.</summary>
    public ThrowContext BuildContext(Rat rat) => new(
        rat,
        GlobalPosition,
        -Player.Camera.GlobalBasis.Z + new Vector3(0, Mathf.DegToRad(Tuning.AngleAdjust), 0),
        _currentForce,
        _gravity * rat.RatDef.Mass,
        Tuning.Step,
        Tuning.MaxPoints
    );

    /// <summary>Blue means the throw feeds the facility, green means it staffs a slot.</summary>
    private Color PathColor(ThrowPath path)
    {
        if (path.ThrowTarget.IsIntake) return IntakeThrowColor;
        return path.Homing ? SlotThrowColor : FreeThrowColor;
    }

    private Tween _chargeTween;
    public async void StartDelayedCharge()
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
        RatCurveState newState = new(rat, _currentPath);
        if (_currentPath.ThrowTarget.IsSlot)
        {
            _currentPath.ThrowTarget.WorkSlot.TryReserve(rat);
        }

        rat.InjectState("throw", newState);
        _isCharging = false;
        ResetCharge();

        EventBus.Publish(Event.RatThrown);
        EventBus.Publish(Event.CameraImpact, chargeAmount, 0.35f);
    }

    private void GenerateMesh()
    {
        _immediateMesh.ClearSurfaces();
        _immediateMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip, _material);

        foreach (Vector3 v in _currentPath.Points)
        {
            _immediateMesh.SurfaceAddVertex(ToLocal(v));
        }

        _immediateMesh.SurfaceEnd();

    }

    private void SetReticle()
    {
        Vector3 reticlePos = _currentPath.End + Vector3.Up * 0.01f;
        Vector3 targetRaycastPos = reticlePos + Vector3.Down;
        if (Utils.Raycast(this, reticlePos, targetRaycastPos, out Dictionary result, 1))
        {
            Vector3 hitNormal = result["normal"].AsVector3();
            Vector3 hitPosition = result["position"].AsVector3();
            Vector3 rotation = _reticleMeshInstance.GlobalRotation;
            rotation.Z = hitNormal.Z;
            rotation.X = hitNormal.X;
            _reticleMeshInstance.GlobalRotation = rotation;
            _reticleMeshInstance.GlobalPosition = hitPosition + Vector3.Up * 0.01f;
        }

    }

    public void Enable()
    {
        _preview = true;
        _reticleMeshInstance.Show();
    }

    public void Reset()
    {
        _preview = false;
        _immediateMesh.ClearSurfaces();
        _reticleMeshInstance.Hide();
        ResetCharge();
    }
}