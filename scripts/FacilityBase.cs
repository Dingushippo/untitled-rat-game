using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class FacilityBase : Node3D
{
    [Export] public Node3D WorkSlotGroup;
    [Export] public Marker3D IntakeMarker;
    [Export] public FacilityDef Facility;
    [Export] public Label3D DebugLabel;
    [Export] public InteractAreaComponent OutputInteract;

    public Inventory Input;
    public Inventory Output;
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
        Input = new Inventory(Facility.BufferSize, Facility.Inputs.Keys);
        Output = new Inventory(Facility.BufferSize);
    }

    public override void _Process(double delta)
    {
        productionComponent.Process(this, (float)delta);
        DebugLabel.Text = $"slots {productionComponent.StaffedSlots}/{Facility.SlotCount} - cycle time: {productionComponent.GetCycleTime(Facility)} - {productionComponent.Buffer}x";
    }

    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        if (rat.Cargo.Total > 0 && Input.Contents.Keys.Any(k => rat.Cargo.Contents.ContainsKey(k)))
        {
            target = new ThrowTarget(this, IntakeMarker);
            return true;
        }
        if (TryGetClosestWorkSlot)
        target = default;
        return false;
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
