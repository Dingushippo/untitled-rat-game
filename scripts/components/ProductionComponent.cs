using Godot;
using System.Linq;

public class ProductionComponent
{
    public const float BUFFER_PENALTY = 3f;
    public float ProductionRate;
    public int StaffedSlots;
    public int Buffer = 0;
    private WorkSlot[] _workSlots;
    private float _timer = 0;

    public ProductionComponent(WorkSlot[] workSlots)
    {
        _workSlots = workSlots;
    }

    public void Process(FacilityBase @base, float delta)
    {
        StaffedSlots = _workSlots.Count(slot => slot.IsOccupied);
        ProductionRate = _workSlots.Sum(slot => slot.Occupant?.RatDef.WorkRate ?? 0) / StaffedSlots;

        if (StaffedSlots == 0) return;

        if (@base.Output.Total > @base.Facility.BufferSize)
        {
            ProductionRate /= BUFFER_PENALTY;
        }
        if (_timer < @base.Facility.CycleSeconds)
        {
            _timer += StaffedSlots * ProductionRate * delta;
            return;
        }
        if (@base.Input.TryRemove(@base.Facility.Inputs))
        {
            @base.Output.Add(@base.Facility.Inputs);
            EventBus.Publish(Event.ProductionCompleted, @base.Facility.Outputs);
        }
        _timer = 0;
    }

    public float GetCycleTime(FacilityDef facility) => facility.CycleSeconds / StaffedSlots * ProductionRate;
}