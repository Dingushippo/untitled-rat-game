using Godot;

public readonly struct ThrowPath
{
    public readonly Vector3[] Points;
    public readonly WorkSlot TargetedSlot;
    public readonly bool Homing;
    public readonly Vector3 End => Points[^1];

    public ThrowPath(Vector3[] points, WorkSlot targetedSlot = null, bool homing = false)
    {
        Points = points;
        TargetedSlot = targetedSlot;
        Homing = homing;
    }
}