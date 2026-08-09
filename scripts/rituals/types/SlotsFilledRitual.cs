using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public partial class SlotsFilledRitual : RitualBase
{
    [Export] public float RitualTime = 10f;

    private bool _isActive;
    private float _timer;

    private readonly List<Tween> _animateTween = new();
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!_isActive) return;
        if (_timer < RitualTime)
        {
            _timer += (float)delta;
            return;
        }
        IsCompleted();
        foreach (Tween tween in _animateTween)
            tween.Kill();
        GD.Print("Ritual Completed");
        _isActive = false;
        foreach (RitualElementSlot slot in Slots)
        {
            slot.WorkSlot.Occupant.ForceState("follow");
        }
    }

    private void AnimateRats()
    {
        float rotation = Mathf.DegToRad(30);
        foreach (Rat rat in Slots.Select(x => x.WorkSlot.Occupant).ToList())
        {
            float startRotation = rat.Rotation.Y;
            Tween tween = CreateTween();
            GD.Print($"Rat {rat} with tween {tween}");
            tween.SetLoops();
            tween.TweenProperty(rat, "rotation:y", startRotation + rotation / 2, 0.2f);
            tween.TweenProperty(rat, "rotation:y", startRotation - rotation / 2, 0.2f);
            _animateTween.Add(tween);
        }
    }


    protected override void CheckSlotsFulfilled()
    {
        if (!Slots.All(x => x.WorkSlot.IsOccupied)) return;

        GD.Print("Ritual started");
        IsStarted();
        AnimateRats();
        _isActive = true;
    }
}