using Godot;
using System.Collections.Generic;

/// <summary>
/// Collects path points together with the speed the rat should be travelling at when it reaches
/// each one. Keeping the two in one place means the simulated ballistic speed survives into
/// playback instead of being flattened to a constant.
/// </summary>
public sealed class ThrowPathBuilder
{
    private readonly List<Vector3> _points = new();
    private readonly List<float> _speeds = new();

    public int Count => _points.Count;

    /// <param name="speed">Speed along the segment that ends at <paramref name="point"/>.</param>
    public void Add(Vector3 point, float speed)
    {
        _points.Add(point);
        _speeds.Add(speed);
    }

    public ThrowPath Build(ThrowTarget target = default, bool homing = false)
        => new(_points.ToArray(), _speeds.ToArray(), target, homing);
}
