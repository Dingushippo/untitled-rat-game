using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public abstract partial class RitualBase : Node3D, IPooledObject
{
    private const float DRAW_FREQ = 60f;
    [Export] public SubViewport Viewport;
    [Export] public RitualRenderer Renderer;
    [Export] public RitualResource RitualResource;
    [Export] public MeshInstance3D PlaneMesh;

    public Array<RitualElementSlot> Slots;
    private Action Completed;
    private Action Started;
    private Action Interrupted;
    private float timer = 0;
    public override void _Ready()
    {
        Renderer.Position = Viewport.Size / 2;
    }

    protected private void IsCompleted()
    {
        Completed?.Invoke();
    }

    protected private void IsStarted()
    {
        Started?.Invoke();
    }

    protected private void IsInterrupted()
    {
        Interrupted?.Invoke();
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
