using Godot;
using System.Linq;

/// <summary>Runs a facility's recipe: staffed slots push a timer, a full cycle swaps Inputs for Outputs.</summary>
public class ProductionComponent
{
    public const float BUFFER_PENALTY = 3f;

    public float ProductionRate;
    public int StaffedSlots;
    public bool IsStalled { get; private set; }

    private readonly WorkSlot[] _workSlots;
    private float _timer = 0;

    public ProductionComponent(WorkSlot[] workSlots)
    {
        _workSlots = workSlots;
    }

    public void Process(FacilityBase @base, float delta)
    {
        FacilityDef def = @base.Facility;

        StaffedSlots = _workSlots.Count(slot => slot.IsOccupied);
        if (StaffedSlots == 0)
        {
            ProductionRate = 0f;
            return;
        }

        // Average worker quality, so one lazy rat doesn't out-weigh the rest of the crew.
        ProductionRate = _workSlots.Sum(slot => slot.Occupant?.RatDef.WorkRate ?? 0f) / StaffedSlots;

        float rate = ProductionRate;
        if (@base.Output.Total >= def.BufferSize * def.BufferPenaltyRatio)
        {
            rate /= BUFFER_PENALTY;
        }

        _timer += StaffedSlots * rate * delta;
        if (_timer < def.CycleSeconds) return;

        // Hold the timer at the gate on failure so work resumes the instant the facility is fed
        // or drained, instead of restarting the whole cycle.
        if (!@base.Input.Has(def.Inputs))
        {
            _timer = def.CycleSeconds;
            SetStalled(@base, Event.ProductionMissingItems, def.Inputs);
            return;
        }
        if (!@base.Output.CanAdd(def.Outputs))
        {
            _timer = def.CycleSeconds;
            SetStalled(@base, Event.ProductionHalted, def.Outputs);
            return;
        }

        @base.Input.TryRemove(def.Inputs);
        @base.Output.TryAdd(def.Outputs);

        _timer = 0f;
        IsStalled = false;
        EventBus.Publish(Event.ProductionCompleted, @base, def.Outputs);
    }

    /// <summary>0-1 progress through the current cycle, for debug readouts and UI.</summary>
    public float GetProgress(FacilityDef facility) =>
        facility.CycleSeconds <= 0f ? 1f : Mathf.Clamp(_timer / facility.CycleSeconds, 0f, 1f);

    private void SetStalled(FacilityBase @base, Event reason, Godot.Collections.Dictionary<string, int> items)
    {
        if (IsStalled) return; // edge-trigger, otherwise this fires every frame
        IsStalled = true;
        EventBus.Publish(reason, @base, items);
    }
}
