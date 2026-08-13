using Godot;
using System.Linq;

[GlobalClass]
public partial class ProductionFacility : FacilityBase
{
    [Export] public Node3D WorkSlotGroup;
    [Export] public InteractAreaComponent OutputInteract;
    public Inventory Input;
    public Inventory Output;
    public ProductionDef ProdFacility;
    private WorkSlot[] _workSlots;
    private ProductionComponent _production;
    private int _lastOutputTotal = -1;
    private const int DEBUG_UPDATES_EVERY_N_SECONDS = 3;
    private float _debugUpdateTimer;



    public override void _Ready()
    {
        base._Ready();
        _workSlots = WorkSlotGroup is null
            ? System.Array.Empty<WorkSlot>()
            : WorkSlotGroup.GetChildren().OfType<WorkSlot>().ToArray();

        ProdFacility = Facility as ProductionDef;
        if (_workSlots.Length != ProdFacility.SlotCount)
        {
            GD.PushWarning($"{Name}: {ProdFacility.Id} declares {ProdFacility.SlotCount} slots but the scene has {_workSlots.Length}");
        }

        if (ProdFacility.HasInputs && IntakeMarker is null)
        {
            GD.PushWarning($"{Name}: {ProdFacility.Id} needs inputs but has no IntakeMarker, so it can never be fed");
        }

        _production = new ProductionComponent(this, _workSlots);

        // A null/empty filter set means the ProdFacility accepts nothing by throw, which is
        // correct for a raw producer - only recipes with inputs have an intake.
        Input = new Inventory(ProdFacility.BufferSize, ProdFacility.Inputs?.Keys ?? Enumerable.Empty<string>());
        Output = new Inventory(ProdFacility.BufferSize);

        if (OutputInteract is not null)
        {
            OutputInteract.OnInteract = OnOutputInteract;
            RefreshOutputPrompt();
        }
    }

    protected override bool WantsCargo(Rat rat)
    {
        return rat.Cargo.HasAnythingFor(Input);
    }

    public override void _Process(double delta)
    {
        _production.Process(this, (float)delta);

        if (Output.Total != _lastOutputTotal)
        {
            _lastOutputTotal = Output.Total;
            RefreshOutputPrompt();
        }

        if (_debugUpdateTimer > DEBUG_UPDATES_EVERY_N_SECONDS)
        {
            UpdateDebugLabel();
            _debugUpdateTimer = 0;
        }
        else
            _debugUpdateTimer += (float)delta;
    }

    /// <summary>Fills the held rat with finished product. Wired to the output interact area.</summary>
    private void OnOutputInteract(Node3D interactor, bool isHeld)
    {
        if (interactor is not Player player) return;

        Rat rat = player.GrabComponent.CurrentGrabbed;
        if (rat is null)
        {
            GD.Print($"{Name}: need a rat in hand to collect from {ProdFacility.DisplayName}");
            return;
        }
        if (Output.IsEmpty)
        {
            GD.Print($"{Name}: {ProdFacility.DisplayName} has nothing to collect");
            return;
        }
        int amount = isHeld ? int.MaxValue : 1;
        InventoryTransfer.Move(Output, rat.Cargo, amount);

        RefreshOutputPrompt();
    }

    public override Godot.Collections.Dictionary<string, int> DeliverCargo(Rat rat)
    {
        var moved = InventoryTransfer.Move(rat.Cargo, Input);
        RefreshOutputPrompt();
        return moved;
    }

    private void RefreshOutputPrompt()
    {
        if (OutputInteract is null) return;

        string contentString = string.Join(", ", Output.Contents.Select(
            k => $"{ItemDatabase.Get(k.Key).DisplayName} x{k.Value}")
        );
        OutputInteract.SetInteractionText(Output.IsEmpty ? "Empty" : $"Collect: {contentString}");
    }

    public override bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = default;
        if (rat is null) return false;

        if (IntakeMarker is not null && rat.Cargo.HasAnythingFor(Input))
        {
            target = ThrowTarget.Intake(IntakeMarker, this);
            return true;
        }

        if (TryGetClosestWorkSlot(from, out WorkSlot slot))
        {
            target = ThrowTarget.Slot(slot, this);
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

    protected override void UpdateDebugLabel()
    {
        base.UpdateDebugLabel();

        if (DebugLabel is null) return;

        string inputString = string.Join(", ", Input.Contents.Select(
            k => $"{ItemDatabase.Get(k.Key).DisplayName} x{k.Value}")
        );
        string outputString = string.Join(", ", Output.Contents.Select(
            k => $"{ItemDatabase.Get(k.Key).DisplayName} x{k.Value}")
        );
        string newText =
            $"{ProdFacility.DisplayName}\n" +
            $"slots {_production.StaffedSlots}/{ProdFacility.SlotCount}  " +
            $"{_production.GetProgress(ProdFacility) * 100f:0}%{(_production.IsStalled ? " (stalled)" : "")}\n" +
            $"in: {inputString}\nout: {outputString}";

        if (newText != DebugLabel.Text)
            DebugLabel.Text = newText;
    }
}