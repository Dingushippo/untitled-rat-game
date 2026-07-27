using Godot;
using System.Collections.Generic;


[GlobalClass]
public abstract partial class ThrowType : Resource
{
    [Export] public float SteeringStrength = 8f;
    [Export] public float ArrivalDistance = 0.05f;
    [Export] public float ApproachHeight = 0.5f;
    [Export] public float CurveStep = 0.05f;
    [Export(PropertyHint.Range, "0, 1.0")] public float TangentStrength = 0.2f;

    public abstract ThrowPath Simulate(ThrowContext ctx);

    protected Vector3 Steer(Vector3 pos, Vector3 vel, Vector3 target, float step)
    {
        Vector3 desired = (target - pos).Normalized() * vel.Length();
        return vel.Lerp(desired, Mathf.Clamp(SteeringStrength * step, 0, 1));
    }

    protected bool HomeTo(ThrowContext ctx, List<Vector3> points, Vector3 pos, Vector3 vel, Vector3 target)
    {
        Vector3 p0 = pos;
        Vector3 p1 = p0 + vel * pos.DistanceTo(target) * 0.2f;
        p1.Y = Mathf.Max(p1.Y, target.Y);
        Vector3 p2 = target + Vector3.Up * ApproachHeight;
        Vector3 p3 = target;

        for (float t = 0; t < 1f; t += CurveStep)
        {
            Vector3 point = p0.BezierInterpolate(p1, p2, p3, t);
            points.Add(point);
        }
        return true;
    }
}