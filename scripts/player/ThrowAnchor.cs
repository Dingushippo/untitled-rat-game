using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

public partial class ThrowAnchor : Node3D
{
    [Export] public Player Player;
    [Export] public Mesh ReticleMesh;
    [Export] public float ThrowForce = 10f;
    [Export] public float AngleAdjust = 0f;
    [Export] public bool Preview = true;
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

    public override void _Ready()
    {
        _gravity = Player.GetGravity();

        _pathMeshInstance = new();
        AddChild(_pathMeshInstance);

        _reticleMeshInstance = new();
        AddChild(_reticleMeshInstance);
        _reticleMeshInstance.Mesh = ReticleMesh;

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
        if (Preview)
        {
            GeneratePath();
            GenerateMesh();
            SetReticle();
        }
    }

    private void GeneratePath()
    {
        List<Vector3> points = new();

        Vector3 position = GlobalPosition;
        Vector3 velocity = -Player.Camera.GlobalBasis.Z * ThrowForce;

        int bounces = 0;

        while (points.Count < MaxPoints)
        {
            velocity += _gravity * Step;

            Vector3 next = position + velocity * Step;

            if (PathCollision(position, next, out Dictionary hit))
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

        // if (points.Count % 2 != 0) points.RemoveAt(points.Count - 1);

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
        if (PathCollision(reticlePos, targetRaycastPos, out Dictionary result))
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
        Preview = true;
    }

    public void Reset()
    {
        Preview = false;
        _pathArray = null;
        _immediateMesh.ClearSurfaces();
    }

    private bool PathCollision(Vector3 a, Vector3 b, out Dictionary result)
    {
        PhysicsDirectSpaceState3D state = Player.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            a, b, 1
        );
        result = state.IntersectRay(query);
        return result.Count != 0;
    }
}