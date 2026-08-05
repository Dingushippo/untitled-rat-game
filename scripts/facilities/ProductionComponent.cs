using Godot;
using System;
using System.Linq;

/// <summary>Runs a facility's recipe: staffed slots push a timer, a full cycle swaps Inputs for Outputs.</summary>
public class ProductionComponent : IDisposable
{
    public float ProductionRate;
    public int StaffedSlots;
    public bool IsStalled { get; private set; }
    private ProductionFacility _facility;
    private readonly WorkSlot[] _workSlots;
    private float _timer = 0;
    private float _cycleTimeScale;
    private bool _stallOverride;

    public ProductionComponent(ProductionFacility facility, WorkSlot[] workSlots)
    {
        _facility = facility;
        _workSlots = workSlots;
        _cycleTimeScale = EconomyService.Instance.CycleTimeScale;
        EventBus.Subscribe(Event.SetDisruptProductionInRange, OnDisruptProductionInRange);
    }

    public void Dispose()
    {
        EventBus.Unsubscribe(Event.SetDisruptProductionInRange, OnDisruptProductionInRange);
    }

    public void OnDisruptProductionInRange(object[] args)
    {
        Vector3 hazardPosition = (Vector3)args[0];
        float hazardRadius = (float)args[1];
        bool disrupt = (bool)args[2];

        if (_facility.GlobalPosition.DistanceTo(hazardPosition) <= hazardRadius)
        {
            _stallOverride = disrupt;
        }
    }

    public void Process(ProductionFacility @base, float delta)
    {
        ProductionDef def = @base.ProdFacility;

        StaffedSlots = _workSlots.Count(slot => slot.IsEntered);
        if (StaffedSlots == 0)
        {
            ProductionRate = 0f;
            return;
        }

        // Average worker quality, so one lazy rat doesn't out-weigh the rest of the crew.
        ProductionRate = _workSlots.Sum(slot => slot.Occupant?.RatDef.WorkRate ?? 0f) / StaffedSlots;

        float rate = ProductionRate * _cycleTimeScale;
        if (@base.Output.Total >= def.BufferSize * def.BufferPenaltyRatio)
        {
            rate /= def.BufferPenalty;
        }

        _timer += StaffedSlots * rate * delta;
        if (_timer < def.CycleSeconds) return;

        // Hold the timer at the gate on failure so work resumes the instant the facility is fed
        // or drained, instead of restarting the whole cycle.
        if (_stallOverride)
        {
            _timer = def.CycleSeconds;
            SetStalled(@base, Event.HazardDisruption);
            return;
        }
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

        _cycleTimeScale = EconomyService.Instance.CycleTimeScale;

        @base.Input.TryRemove(def.Inputs);
        @base.Output.TryAdd(def.Outputs);
        EventBus.Publish(Event.ProductionCompleted, @base, def.Outputs);

        _timer = 0f;
        IsStalled = false;
    }

    /// <summary>0-1 progress through the current cycle, for debug readouts and UI.</summary>
    public float GetProgress(ProductionDef facility) =>
        facility.CycleSeconds <= 0f ? 1f : Mathf.Clamp(_timer / facility.CycleSeconds, 0f, 1f);

    private void SetStalled(ProductionFacility @base, Event reason, Godot.Collections.Dictionary<string, int> items = null)
    {
        if (IsStalled) return; // edge-trigger, otherwise this fires every frame
        IsStalled = true;
        EventBus.Publish(reason, @base, items);
    }
}
