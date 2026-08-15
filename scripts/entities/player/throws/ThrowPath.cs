using Godot;

public readonly struct ThrowPath
{
    public readonly Vector3[] Points;
    public readonly float[] Speeds;
    public readonly byte[] Segments;
    public readonly int[] Impacts;
    public readonly float Length;
    public readonly ThrowTarget ThrowTarget;
    public readonly bool Homing;
    public readonly Vector3 End => Points[^1];
    public readonly Vector3 ExitVelocity;

    public ThrowPath(Vector3[] points, float[] speeds, byte[] segments, int[] impacts, float length, Vector3 exitVelocity, ThrowTarget throwTarget = default, bool homing = false)
    {
        Points = points;
        Speeds = speeds;
        Segments = segments;
        Impacts = impacts;
        Length = length;
        ExitVelocity = exitVelocity;
        ThrowTarget = throwTarget;
        Homing = homing;
    }
}