using Godot;
using System;
using System.ComponentModel;

[GlobalClass, Tool]
public partial class RitualBase : Node3D
{
    // Called when the node enters the scene tree for the first time.
    [Export] public SubViewport Viewport;
    [Export] public RitualRenderer Renderer;
    [Export] public RitualResource RitualResource;
    [Export] public MeshInstance3D PlaneMesh;
    public override void _Ready()
    {
        Renderer.Position = Viewport.Size / 2;
        Renderer.QueueRedraw();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
