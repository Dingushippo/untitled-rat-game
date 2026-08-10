using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class RitualBase : Node3D, IPooledObject
{
    private const float DRAW_FREQ = 60f;
    [Export] public SubViewport Viewport;
    [Export] public RitualRenderer Renderer;
    [Export] public RitualResource RitualResource;
    [Export] public MeshInstance3D PlaneMesh;

    private Array<RitualElementSlot> _slots;
    public Array<RitualElementSlot> Slots
    {
        get => _slots;
        set
        {
            _slots = value;
            // foreach (RitualElementSlot slot in Slots)
                // slot.Fulfilled += CheckSlotsFulfilled;
            Renderer.QueueRedraw();
        }
    }

    private Action Completed;
    private Action Started;
    private Action Interrupted;
    private float timer = 0;
    private bool _isActive;
    private readonly List<Tween> _animateTween = new();

    public override void _Ready()
    {
        Renderer.Position = Viewport.Size / 2;
    }

    protected private void IsCompleted()
    {
        _isActive = false;
        Completed?.Invoke();

        foreach (RitualElementSlot slot in Slots)
        {
            slot.WorkSlot.Occupant.ForceState("follow");
        }
    }

    protected private void IsStarted()
    {
        AnimateRats();
        _isActive = true;
        Started?.Invoke();
    }

    protected private void IsInterrupted()
    {
        Interrupted?.Invoke();
    }

    public override void _Process(double delta)
    {
        if (!_isActive) return;
        if (timer > RitualResource.RitualTime)
        {
            timer += (float)delta;
            return;
        }
        IsCompleted();
        // Renderer.QueueRedraw();
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

        if (Slots == null) return;

        foreach (RitualElementSlot slot in Slots)
        {
            // slot.Fulfilled -= CheckSlotsFulfilled;
        }
    }

    public void AnimateRats()
    {
        float rotation = Mathf.DegToRad(30);
        foreach (Rat rat in Slots.Select(x => x.WorkSlot.Occupant).ToList())
        {
            float startRotation = rat.Rotation.Y;
            Tween tween = CreateTween();
            tween.SetLoops();
            tween.TweenProperty(rat, "rotation:y", startRotation + rotation / 2, 0.2f);
            tween.TweenProperty(rat, "rotation:y", startRotation - rotation / 2, 0.2f);
            _animateTween.Add(tween);
        }
    }

    public void StopAnimation() => _animateTween.ForEach(x => x.Kill());

    // protected abstract void CheckSlotsFulfilled();
}
