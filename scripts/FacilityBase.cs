using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class FacilityBase : Node3D
{
    // Called when the node enters the scene tree for the first time.
    [Export] public Node3D WorkSlotGroup;
    [Export] public FacilityDef Facility;
    [Export] public Label3D DebugLabel;

    private WorkSlot[] _workSlots;
    private ProductionComponent productionComponent;


    public override void _Ready()
    {
        if (Facility is null)
        {
            GD.PrintErr($"{Name} is missing a facility definition");
        }
        Node[] nodes = WorkSlotGroup.GetChildren().ToArray();
        _workSlots = nodes.Cast<WorkSlot>().ToArray();
        productionComponent = new ProductionComponent(_workSlots);
    }

    public override void _Process(double delta)
    {
        productionComponent.Process(Facility, (float)delta);
        DebugLabel.Text = $"slots {productionComponent.StaffedSlots}/{Facility.SlotCount} - cycle time: {productionComponent.GetCycleTime(Facility)} - {productionComponent.Buffer}x";
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
