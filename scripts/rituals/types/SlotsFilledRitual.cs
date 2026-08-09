using Godot;
using System.Linq;

public partial class SlotsFilledRitual : RitualBase
{
    [Export] public float RitualTime = 10f;

    private bool _isActive;
    private float _timer;

    public override void _Ready()
    {
        base._Ready();

        foreach (RitualElementSlot slot in Slots)
        {
            slot.Fulfilled += CheckSlotsFulfilled;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!_isActive) return;
        if (_timer < RitualTime)
        {
            _timer += (float)delta;
        }
        IsCompleted();
        GD.Print("Ritual Completed");
        _isActive = false;
    }


    private void CheckSlotsFulfilled()
    {
        if (!Slots.All(x => x.WorkSlot.IsOccupied)) return;

        GD.Print("Ritual started");
        IsStarted();
        _isActive = true;
    }
}