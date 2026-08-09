using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[GlobalClass, Tool]
public abstract partial class RitualBase : Node3D, IPooledObject
{
    private const float DRAW_FREQ = 60f;
    // Called when the node enters the scene tree for the first time.
    [Export] public SubViewport Viewport;
    [Export] public RitualRenderer Renderer;
    [Export] public RitualResource RitualResource;
    [Export] public MeshInstance3D PlaneMesh;
    [Export] public PackedScene SlotScene;

    private float timer = 0;
    public override void _Ready()
    {
        Renderer.Position = Viewport.Size / 2;
    }
    public override void _Process(double delta)
    {
        if (timer > DRAW_FREQ)
        {
            timer += (float)delta;
            return;
        }
        Renderer.QueueRedraw();
    }

    public void OnSpawn()
    {
        Show();
        SetProcess(true);
        SetPhysicsProcess(true);
    }

    public void OnDespawn()
    {
        Hide();
        SetProcess(false);
        SetPhysicsProcess(false);
    }
}
