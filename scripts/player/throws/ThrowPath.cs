using Godot;

public readonly struct ThrowPath
{
    public readonly Vector3[] Points;
    public readonly ThrowTarget ThrowTarget;
    public readonly bool Homing;
    public readonly Vector3 End => Points[^1];

    public ThrowPath(Vector3[] points, ThrowTarget throwTarget = default, bool homing = false)
    {
        Points = points;
        ThrowTarget = throwTarget;
        Homing = homing;
    }
}