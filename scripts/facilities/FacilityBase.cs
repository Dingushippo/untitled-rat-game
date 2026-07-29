using Godot;
using System.Linq;

public partial class FacilityBase : StaticBody3D
{
    [Export] public Node3D WorkSlotGroup;
    [Export] public Marker3D IntakeMarker;
    [Export] public FacilityDef Facility;
    [Export] public Label3D DebugLabel;
    [Export] public InteractAreaComponent OutputInteract;

    /// <summary>Ingredient buffer, filtered to the recipe's inputs.</summary>
    public Inventory Input;

    /// <summary>Product buffer, drained by carrying a rat to the output interact area.</summary>
    public Inventory Output;

    private WorkSlot[] _workSlots;
    private ProductionComponent _production;
    private int _lastOutputTotal = -1;

    /// <summary>
    /// World Y of the top of this facility's collision shapes. Homing throws lift their approach
    /// above this so the curve arcs over the structure instead of clipping through it.
    /// </summary>
    public float ColliderTopY { get; private set; }

    public override void _Ready()
    {
        if (Facility is null)
        {
            GD.PrintErr($"{Name} is missing a facility definition");
            SetProcess(false);
            return;
        }

        _workSlots = WorkSlotGroup is null
            ? System.Array.Empty<WorkSlot>()
            : WorkSlotGroup.GetChildren().OfType<WorkSlot>().ToArray();

        if (_workSlots.Length != Facility.SlotCount)
        {
            GD.PushWarning($"{Name}: {Facility.Id} declares {Facility.SlotCount} slots but the scene has {_workSlots.Length}");
        }

        _production = new ProductionComponent(_workSlots);
        ColliderTopY = ComputeColliderTopY();

        // A null/empty filter set means the facility accepts nothing by throw, which is
        // correct for a raw producer - only recipes with inputs have an intake.
        Input = new Inventory(Facility.BufferSize, Facility.Inputs?.Keys ?? Enumerable.Empty<string>());
        Output = new Inventory(Facility.BufferSize);

        if (Facility.HasInputs && IntakeMarker is null)
        {
            GD.PushWarning($"{Name}: {Facility.Id} needs inputs but has no IntakeMarker, so it can never be fed");
        }

        if (OutputInteract is not null)
        {
            OutputInteract.OnInteract = OnOutputInteract;
            RefreshOutputPrompt();
        }
    }

    public override void _Process(double delta)
    {
        _production.Process(this, (float)delta);

        if (Output.Total != _lastOutputTotal)
        {
            _lastOutputTotal = Output.Total;
            RefreshOutputPrompt();
        }

        UpdateDebugLabel();
    }

    /// <summary>
    /// Picks what a thrown rat should home onto: the intake if it is carrying something the
    /// recipe wants, otherwise a free work slot.
    /// </summary>
    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = default;
        if (rat is null) return false;

        if (IntakeMarker is not null && rat.Cargo.HasAnythingFor(Input))
        {
            target = ThrowTarget.Intake(this, IntakeMarker);
            return true;
        }

        if (TryGetClosestWorkSlot(from, out WorkSlot slot))
        {
            target = ThrowTarget.Slot(this, slot);
            return true;
        }

        return false;
    }

    public bool TryGetClosestWorkSlot(Vector3 from, out WorkSlot slot)
    {
        WorkSlot closest = null;
        float closestDistanceSq = float.MaxValue;

        foreach (WorkSlot s in _workSlots)
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
        return closest != null;
    }

    /// <summary>Unloads a rat's cargo into the ingredient buffer. Called when an intake throw lands.</summary>
    public Godot.Collections.Dictionary<string, int> DeliverCargo(Rat rat)
    {
        var moved = InventoryTransfer.Move(rat.Cargo, Input);
        RefreshOutputPrompt();
        return moved;
    }

    /// <summary>Fills the held rat with finished product. Wired to the output interact area.</summary>
    private void OnOutputInteract(Node3D interactor)
    {
        if (interactor is not Player player) return;

        Rat rat = player.GrabComponent.CurrentGrabbed;
        if (rat is null)
        {
            GD.Print($"{Name}: need a rat in hand to collect from {Facility.DisplayName}");
            return;
        }
        if (Output.IsEmpty)
        {
            GD.Print($"{Name}: {Facility.DisplayName} has nothing to collect");
            return;
        }

        InventoryTransfer.Move(Output, rat.Cargo);
        RefreshOutputPrompt();
    }

    /// <summary>Highest point of any owned collision shape, in world space.</summary>
    private float ComputeColliderTopY()
    {
        float top = GlobalPosition.Y;

        foreach (Node child in GetChildren())
        {
            if (child is not CollisionShape3D shapeNode || shapeNode.Disabled || shapeNode.Shape is null)
                continue;

            Mesh debugMesh = shapeNode.Shape.GetDebugMesh();
            if (debugMesh is null) continue;

            Aabb bounds = shapeNode.GlobalTransform * debugMesh.GetAabb();
            top = Mathf.Max(top, bounds.End.Y);
        }

        return top;
    }

    private void RefreshOutputPrompt()
    {
        if (OutputInteract is null) return;

        string contentString = string.Join(", ", Output.Contents.Select(
            k => $"{ItemDatabase.Get(k.Key).DisplayName} x{k.Value}")
        );
        OutputInteract.SetInteractionText(Output.IsEmpty ? "Empty" : $"Collect: {contentString}");
    }

    private void UpdateDebugLabel()
    {
        if (DebugLabel is null) return;

        string inputString = string.Join(", ", Input.Contents.Select(
            k => $"{ItemDatabase.Get(k.Key).DisplayName} x{k.Value}")
        );
        string outputString = string.Join(", ", Output.Contents.Select(
            k => $"{ItemDatabase.Get(k.Key).DisplayName} x{k.Value}")
        );
        DebugLabel.Text =
            $"{Facility.DisplayName}\n" +
            $"slots {_production.StaffedSlots}/{Facility.SlotCount}  " +
            $"{_production.GetProgress(Facility) * 100f:0}%{(_production.IsStalled ? " (stalled)" : "")}\n" +
            $"in: {inputString}\nout: {outputString}";
    }
}
