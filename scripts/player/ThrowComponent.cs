using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class ThrowComponent : Node3D
{
    [Export] public Player Player;
    [Export] public Mesh ReticleMesh;
    [Export] public ThrowType ThrowType;
    [Export] public float ThrowForce = 10f;
    [Export] public float MaxThrowForce = 30f;
    [Export] public float ChargeDuration = 2f;
    [Export] public float ChargeStartDelay = 0.2f;
    [Export] public float AngleAdjust = 0f;
    [Export] public float Step = 0.02f;
    [Export] public float BounceDecay = 0.5f;
    [Export] public int MaxBounces = 1;
    [Export] public int MaxPoints = 1000;


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
        _gravity = Player.GetGravity();
        _currentForce = ThrowForce;

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
            ThrowContext ctx = new ThrowContext(
                this,
                GlobalPosition,
                -Player.Camera.GlobalBasis.Z + new Vector3(0, Mathf.DegToRad(AngleAdjust), 0),
                _currentForce,
                _gravity,
                Step,
                MaxPoints
            );
            _currentPath = ThrowType.Simulate(ctx);
            _material.AlbedoColor = _currentPath.Homing ? Colors.Green : Colors.Red;

            GenerateMesh();
            SetReticle();
        }
    }

    private Tween _chargeTween;
    public async void StartDelayedCharge()
    {
        _chargeTween = CreateTween();
        _chargeTween.TweenMethod(
            Callable.From<float>(v => _currentForce = v),
            ThrowForce,
            MaxThrowForce,
            ChargeDuration
        ).SetDelay(ChargeStartDelay);
        _isCharging = true;

        EventBus.Publish(Event.CameraCharge, ChargeDuration, ChargeStartDelay);
    }

    public void ResetCharge()
    {
        if (_chargeTween != null)
        {
            _chargeTween.Kill();
        }
        _currentForce = ThrowForce;

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
        float chargeAmount = Mathf.IsEqualApprox(MaxThrowForce, ThrowForce)
            ? 1f
            : Mathf.Clamp((_currentForce - ThrowForce) / (MaxThrowForce - ThrowForce), 0f, 1f);

        float curveSpeed = Mathf.Remap(
            _currentForce,
            ThrowForce,
            MaxThrowForce,
            RatCurveState.MIN_SPEED,
            RatCurveState.MAX_SPEED
        );
        GD.Print($"Targeted slot: {_currentPath.TargetedSlot}");
        RatCurveState newState = new(rat, _currentPath.Points, curveSpeed, _currentPath.TargetedSlot);
        if (_currentPath.TargetedSlot != null)
        {
            _currentPath.TargetedSlot.TryReserve(rat);
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