using Godot;

public readonly struct ThrowPath
{
    public readonly Vector3[] Points;

    /// <summary>
    /// Simulated speed in m/s for the segment ending at the matching entry in <see cref="Points"/>.
    /// Same length as <see cref="Points"/>.
    /// </summary>
    public readonly float[] Speeds;

    public readonly ThrowTarget ThrowTarget;
    public readonly bool Homing;
    public readonly Vector3 End => Points[^1];

    public readonly Vector3 ExitVelocity;

    public ThrowPath(Vector3[] points, float[] speeds, Vector3 exitVelocity, ThrowTarget throwTarget = default, bool homing = false)
    {
        Points = points;
        Speeds = speeds;
        ExitVelocity = exitVelocity;
        ThrowTarget = throwTarget;
        Homing = homing;
    }
}