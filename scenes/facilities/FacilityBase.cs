using Godot;
using System;
using System.ComponentModel;

public partial class FacilityBase : Node3D
{
    // Called when the node enters the scene tree for the first time.
    [Export] public Node3D WorkSlots;
    public override void _Ready()
    {
    }

    public Node3D GetClosestWorkSlot(Vector3 from)
    {
        Node3D closest = null;
        float closestDistanceSq = float.MaxValue;

        foreach (Node child in WorkSlots.GetChildren())
        {
            if (child is not Node3D slot)
                continue;

            float distSq = from.DistanceSquaredTo(slot.GlobalPosition);

            if (distSq < closestDistanceSq)
            {
                closestDistanceSq = distSq;
                closest = slot;
            }
        }

        return closest;
    }
}
