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
        GD.Print("Ritual Completed");
        _isActive = false;
        foreach (RitualElementSlot slot in Slots)
        {
            slot.WorkSlot.Occupant.ForceState("follow");
        }
    }


    // protected override void CheckSlotsFulfilled()
    // {
    //     if (!Slots.All(x => x.WorkSlot.IsOccupied)) return;

    //     GD.Print("Ritual started");
    //     IsStarted();
    //     AnimateRats();
    //     _isActive = true;
    // }
}