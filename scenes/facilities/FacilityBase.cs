using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class FacilityBase : Node3D
{
    // Called when the node enters the scene tree for the first time.
    [Export] public Node3D WorkSlotGroup;

    private WorkSlot[] _workSlots;


    public override void _Ready()
    {
        Node[] nodes = WorkSlotGroup.GetChildren().ToArray();
        _workSlots = nodes.Cast<WorkSlot>().ToArray();
    }

    public bool TryGetClosestWorkSlot(Vector3 from, out WorkSlot slot)
    {
        WorkSlot closest = null;
        float closestDistanceSq = float.MaxValue;

        foreach (WorkSlot s in WorkSlotGroup.GetChildren())
        {
            if (s.IsOccupied) continue;

            float distSq = from.DistanceSquaredTo(s.GlobalPosition);

            if (distSq < closestDistanceSq)
            {
                closestDistanceSq = distSq;
                closest = s;
            }
        }
        
        slot = closest;
        if (closest == null)
        {
            return false;
        }
        return true;
        
    }
}
