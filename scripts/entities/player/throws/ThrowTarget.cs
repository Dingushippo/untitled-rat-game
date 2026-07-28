using Godot;
public enum ThrowTargetKind {Slot, Intake}

public readonly struct ThrowTarget
{
    public readonly ThrowTargetKind Kind;
    public readonly FacilityBase Facility;
    public readonly Node3D Anchor;
    public readonly WorkSlot WorkSlot;
    public Vector3 Position => Anchor.GlobalPosition;

    /// <summary>A default ThrowTarget means "no target"; a real one always has a facility.</summary>
    public bool IsValid => Facility != null && Anchor != null;
    public bool IsSlot => IsValid && Kind == ThrowTargetKind.Slot;
    public bool IsIntake => IsValid && Kind == ThrowTargetKind.Intake;

    private ThrowTarget(
        ThrowTargetKind kind,
        FacilityBase facility,
        Node3D anchor,
        WorkSlot slot)
    {
        Kind = kind;
        Facility = facility;
        Anchor = anchor;
        WorkSlot = slot;
    }

    public static ThrowTarget Intake(FacilityBase facility, Node3D anchor)
        => new(
            ThrowTargetKind.Intake,
            facility,
            anchor,
            null);

    public static ThrowTarget Slot(FacilityBase facility, WorkSlot slot)
        => new(
            ThrowTargetKind.Slot,
            facility,
            slot,
            slot);
}