using Godot;
using System;
using System.Linq;

[GlobalClass, Tool]
public partial class RatSpawner : Node3D
{
    [Export] public PackedScene RatScene;
    [Export] public RatDef ratDef;
    [Export] public float InnerRadius = 1f;
    [Export] public float OuterRadius = 2f;
    [Export] bool Debug;

    private int _ratCounter = 0;
    private MeshInstance3D _debugMesh;
    private ImmediateMesh _mesh = new();
    private OrmMaterial3D _material = new();

    public override void _EnterTree()
    {
        EventBus.Subscribe<SpawnRat>(OnSpawnRat);
    }
    public override void _ExitTree()
    {
        EventBus.Unsubscribe<SpawnRat>(OnSpawnRat);
    }
    public override void _Ready()
    {
        if (GetChildOrNull<MeshInstance3D>(0) != null)
        {
            return;
        }
        _debugMesh = new MeshInstance3D
        {
            Mesh = _mesh
        };
        AddChild(_debugMesh);
        _material.AlbedoColor = Colors.Red;
        _debugMesh.Owner = GetTree().EditedSceneRoot;

    }

    public override void _Process(double delta)
    {
        // if (Engine.IsEditorHint())
        UpdateDebugMesh();
    }

    public void OnSpawnRat(SpawnRat evt)
    {
        for (int i = 0; i < evt.Amount; i++)
        {
            Rat rat = RatScene.Instantiate<Rat>();
            Vector3 SpawnPoint = GetRandomSpawnPoint();
            AddChild(rat);
            rat.RatDef = ratDef;
            rat.GlobalPosition = SpawnPoint;
            rat.HomePosition = SpawnPoint;
            _ratCounter++;
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        float radius = (float)GD.RandRange(InnerRadius, OuterRadius);
        float rotation = GD.Randf() * Mathf.Tau;
        return GlobalPosition + Vector3.Forward.Rotated(Vector3.Up, rotation) * radius;
    }

    private void UpdateDebugMesh()
    {
        if (!Debug) return;
        _mesh.ClearSurfaces();
        DrawDebugCircle(InnerRadius, 0.02f);
        DrawDebugCircle(OuterRadius, 0.02f);
    }

    private void DrawDebugCircle(float radius, float step)
    {
        _mesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip, _material);
        for (float theta = 0; theta < Mathf.Tau; theta += step)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            _mesh.SurfaceAddVertex(new Vector3(x, GlobalPosition.Y + 0.3f, z));
        }
        _mesh.SurfaceEnd();
    }
}