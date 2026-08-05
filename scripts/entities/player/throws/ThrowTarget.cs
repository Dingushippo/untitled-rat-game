using Godot;
public enum ThrowTargetKind { Slot, Intake, Other }

public readonly struct ThrowTarget
{
    public readonly ThrowTargetKind Kind;
    public readonly FacilityBase Facility;
    public readonly float ColliderTopY;
    // public readonly Vector3 OverrideRotation;
    public readonly Node3D Anchor;
    public Vector3 Position => Anchor.GlobalPosition;
    public WorkSlot WorkSlot => Anchor as WorkSlot;

    public bool IsValid => Anchor != null;
    public bool IsSlot => IsValid && Kind == ThrowTargetKind.Slot;
    public bool IsIntake => IsValid && Kind == ThrowTargetKind.Intake;
    public bool IsOther => IsValid && Kind == ThrowTargetKind.Other;

    private ThrowTarget(
        ThrowTargetKind kind,
        FacilityBase facility,
        Node3D anchor,
        float colliderTopY = 0
    // Vector3 overrideRotation = new Vector3()
    )
    {
        Kind = kind;
        Facility = facility;
        Anchor = anchor;
        ColliderTopY = colliderTopY;
        // OverrideRotation = overrideRotation;
    }

    public static ThrowTarget Intake(Node3D anchor, FacilityBase facility)
        => new(
            ThrowTargetKind.Intake,
            facility,
            anchor,
            facility.ColliderTopY
        );

    public static ThrowTarget Slot(WorkSlot slot, FacilityBase facility = null)
        => new(
            ThrowTargetKind.Slot,
            facility,
            slot,
            facility != null ? facility.ColliderTopY : 0f
        );
}