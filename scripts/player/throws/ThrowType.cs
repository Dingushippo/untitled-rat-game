using Godot;
using System.Collections.Generic;


[GlobalClass]
public abstract partial class ThrowType : Resource
{
    [Export] public float SteeringStrength = 8f;
    [Export] public float ArrivalDistance = 0.05f;
    public abstract ThrowPath Simulate(ThrowContext ctx);

    protected Vector3 Steer(Vector3 pos, Vector3 vel, Vector3 target, float step)
    {
        Vector3 desired = (target - pos).Normalized() * vel.Length();
        return vel.Lerp(desired, Mathf.Clamp(SteeringStrength * step, 0, 1));
    }

    protected bool HomeTo(ThrowContext ctx, List<Vector3> points, Vector3 pos, Vector3 vel, Vector3 target)
    {
        while (pos.DistanceTo(target) > ArrivalDistance && points.Count < ctx.MaxPoints)
        {
            vel = Steer(pos, vel, target, ctx.Step);
            pos += vel * ctx.Step;
            points.Add(pos);
        }
        if (pos.DistanceTo(target) <= ArrivalDistance)
        {
            points.Add(target);
            return true;
        }
        return false;
    }
}