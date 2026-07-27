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

    public void Process(FacilityDef facility, float delta)
    {
        StaffedSlots = _workSlots.Count(slot => slot.IsOccupied);
        ProductionRate = _workSlots.Sum(slot => slot.Occupant?.RatDef.WorkRate ?? 0) / StaffedSlots;

        if (StaffedSlots == 0) return;

        if (Buffer > facility.BufferSize)
        {
            ProductionRate /= BUFFER_PENALTY;
        }
        if (_timer < facility.CycleSeconds)
        {
            _timer += StaffedSlots * ProductionRate * delta;
            return;
        }
        Buffer += facility.Outputs.Sum(f => f.Value);
        EventBus.Publish(Event.ProductionCompleted, facility.Outputs);
        _timer = 0;
    }

    public float GetCycleTime(FacilityDef facility) => facility.CycleSeconds / StaffedSlots * ProductionRate;
}