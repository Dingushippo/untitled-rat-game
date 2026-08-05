using Godot;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Collects path points together with the speed the rat should be travelling at when it reaches
/// each one. Keeping the two in one place means the simulated ballistic speed survives into
/// playback instead of being flattened to a constant.
/// </summary>
public sealed class ThrowPathBuilder
{
    private readonly List<Vector3> _points = new();
    private readonly List<float> _speeds = new();
    private readonly List<byte> _segments = new();
    private readonly List<int> _impacts = new();
    private float _length = 0;

    public byte CurrentSegment = 0;
    public Vector3 ExitVelocity;

    public int Count => _points.Count;

    /// <param name="speed">Speed along the segment that ends at <paramref name="point"/>.</param>
    public void Add(Vector3 point, float speed)
    {
        if (_points.Count > 0)
            _length += _points[^1].DistanceTo(point);
        _points.Add(point);
        _speeds.Add(speed);
        _segments.Add(CurrentSegment);
    }

    public void AddImpact()
    {
        _impacts.Add(_points.Count - 1);
    }


    public ThrowPath Build(ThrowTarget target = default, bool homing = false)
    {

        return new(
            _points.ToArray(),
            _speeds.ToArray(),
            _segments.ToArray(),
            _impacts.ToArray(),
            _length,
            ExitVelocity,
            target,
            homing
        );
    }
}
