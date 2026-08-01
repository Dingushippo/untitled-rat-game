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
        GD.Print("Entered tree");
        EventBus.Subscribe(Event.SpawnRat, OnSpawnRat);
    }
    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.SpawnRat, OnSpawnRat);
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

    public void OnSpawnRat(params object[] args)
    {
        int amount = args is not [] ? (int)args[0] : 1;

        for (int i = 0; i < amount; i++)
        {
            Rat rat = RatScene.Instantiate<Rat>();
            AddChild(rat);
            rat.RatDef = ratDef;
            rat.GlobalPosition = GetRandomSpawnPoint();
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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Z && GameManager.Instance.Tuning.DebugKeys)
        {
            OnSpawnRat();
        }
    }
}