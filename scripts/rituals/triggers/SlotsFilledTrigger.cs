using Godot.Collections;
using System;
using System.Linq;

public class SlotsFilledTrigger : IRitualTrigger
{
    public Action OnFulfilled { get; set; }

    public bool IsFulfilled => _slots.All(x => x.WorkSlot.IsEntered);

    private Array<RitualElementSlot> _slots;

    public SlotsFilledTrigger(Array<RitualElementSlot> slots)
    {
        _slots = slots;
        foreach (RitualElementSlot slot in slots)
        {
            slot.WorkSlot.Entered += CheckSlots;
        }
    }

    public void CheckSlots()
    {
        if (IsFulfilled)
            OnFulfilled?.Invoke();
    }
}