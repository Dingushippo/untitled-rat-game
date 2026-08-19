using System;
using System.Linq;
using Godot;

/// <summary>Runs a facility's recipe: staffed slots push a timer, a full cycle swaps Inputs for Outputs.</summary>
public class ProductionComponent : IDisposable
{
    public float ProductionRate;

    private StallStatus _currentStall;
    public StallStatus CurrentStall
    {
        get => _currentStall;
        set
        {
            if (value != CurrentStall)
                OnStallChange?.Invoke(value);
            _currentStall = value;
        }
    }
    public Action<StallStatus> OnStallChange;
    public int StaffedSlots;
    public bool IsStalled { get; private set; }
    private ProductionFacility _facility;
    private readonly WorkSlot[] _workSlots;
    private float _timer = 0;
    private bool _stallOverride;

    public ProductionComponent(ProductionFacility facility, WorkSlot[] workSlots)
    {
        _facility = facility;
        _workSlots = workSlots;
        EventBus.Subscribe<SetDisruptFacilityInRange>(OnDisruptProductionInRange);
        EventBus.Subscribe<RatSlotChange>(OnSlotChange);
    }

    private void OnSlotChange(RatSlotChange evt)
    {
        if (evt.Facility != _facility)
            return;

        StaffedSlots += evt.Slotted ? 1 : -1;

        if (StaffedSlots == 0)
            ProductionRate = 0;
        else
            ProductionRate =
                _workSlots.Sum(slot => slot.Occupant?.RatDef.WorkRate ?? 0f) / StaffedSlots;
    }

    public void Dispose()
    {
        EventBus.Unsubscribe<SetDisruptFacilityInRange>(OnDisruptProductionInRange);
        EventBus.Unsubscribe<RatSlotChange>(OnSlotChange);
    }

    public void OnDisruptProductionInRange(SetDisruptFacilityInRange evt)
    {
        GD.Print($"{_facility} stall event {evt}");
        if (_facility.GlobalPosition.DistanceTo(evt.Position) <= evt.Radius)
        {
            _stallOverride = evt.Disrupt;
        }
    }

    public void Process(ProductionFacility @base, float delta)
    {
        ProductionDef def = @base.ProdFacility;

        float rate = ProductionRate * EconomyService.Instance.ProductionRateScale;
        if (@base.Output.Total >= def.BufferSize * def.BufferPenaltyRatio)
        {
            rate /= def.BufferPenalty;
        }
        if (_stallOverride)
        {
            CurrentStall = StallStatus.Hazard;
            return;
        }
        if (StaffedSlots == 0)
        {
            CurrentStall = StallStatus.NoWorkers;
            return;
        }
        if (!@base.Input.Has(def.Inputs))
        {
            CurrentStall = StallStatus.NoInput;
            return;
        }

        if (!@base.Output.CanAdd(def.Outputs))
        {
            CurrentStall = StallStatus.OutputFull;
            return;
        }

        CurrentStall = StallStatus.None;

        _timer += StaffedSlots * rate * delta;
        if (_timer < def.CycleSeconds)
            return;

        @base.Input.TryRemove(def.Inputs);
        @base.Output.TryAdd(def.Outputs);

        _timer = 0f;
        IsStalled = false;
    }

    public float GetProgress(ProductionDef facility) =>
        facility.CycleSeconds <= 0f ? 1f : Mathf.Clamp(_timer / facility.CycleSeconds, 0f, 1f);
}
