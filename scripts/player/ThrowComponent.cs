using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

public partial class ThrowComponent : Node3D
{
    [Export] public Player Player;
    [Export] public Mesh ReticleMesh;
    [Export] public float ThrowForce = 10f;
    [Export] public float MaxThrowForce = 30f;
    [Export] public float ChargeSpeed = 2f;
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
    private Vector3[] _pathArray;
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
            GeneratePath();
            GenerateMesh();
            SetReticle();
        }
    }

    public override void _Process(double delta)
    {
        if (_isCharging)
        {
            float chargeFactor = (MaxThrowForce - ThrowForce) / ChargeSpeed;
            _currentForce = Mathf.Clamp(_currentForce + (float)delta * chargeFactor, ThrowForce, MaxThrowForce);
        }
    }

    public async void StartDelayedCharge()
    {
        await ToSignal(GetTree().CreateTimer(ChargeStartDelay), SceneTreeTimer.SignalName.Timeout);
        _isCharging = true;
    }

    public void ResetCharge()
    {
        _currentForce = ThrowForce;
        _isCharging = false;
    }
    public void Throw(Rat rat)
    {
        float curveSpeed = Mathf.Remap(
            _currentForce,
            ThrowForce,
            MaxThrowForce,
            RatCurveState.MIN_SPEED,
            RatCurveState.MAX_SPEED
        );
        RatCurveState newState = new(rat, _pathArray, curveSpeed);
        rat.InjectState("throw", newState);

        ResetCharge();
    }

    private void GeneratePath()
    {
        List<Vector3> points = new();

        Vector3 position = GlobalPosition;
        Vector3 velocity = (-Player.Camera.GlobalBasis.Z + new Vector3(0, Mathf.DegToRad(AngleAdjust), 0)) * _currentForce;

        int bounces = 0;

        while (points.Count < MaxPoints)
        {
            velocity += _gravity * Step;

            Vector3 next = position + velocity * Step;

            if (Utils.Raycast(this, position, next, out Dictionary hit, 1))
            {
                Vector3 newPos = hit["position"].AsVector3();
                points.Add(newPos);

                velocity = velocity.Bounce(hit["normal"].AsVector3()) * BounceDecay;
                position = newPos;

                if (++bounces > MaxBounces)
                    break;
            }
            else
            {
                points.Add(next);
                position = next;
            }
        }
        _pathArray = points.ToArray();
    }

    private void GenerateMesh()
    {
        _immediateMesh.ClearSurfaces();
        _immediateMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip, _material);

        foreach (Vector3 v in _pathArray)
        {
            _immediateMesh.SurfaceAddVertex(ToLocal(v));
        }

        _immediateMesh.SurfaceEnd();

    }

    private void SetReticle()
    {
        Vector3 reticlePos = _pathArray[_pathArray.Length - 1] + Vector3.Up * 0.01f;
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
        _pathArray = null;
        _immediateMesh.ClearSurfaces();
        _reticleMeshInstance.Hide();
        ResetCharge();
    }
}