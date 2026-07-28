using Godot;
using Godot.Collections;
using System.Collections.Generic;


[GlobalClass]
public partial class SimpleThrow : ThrowType
{
    [Export] public float BounceDecay = 0.8f;
    [Export] public int MaxBounces = 1;
    public override ThrowPath Simulate(ThrowContext ctx)
    {
        List<Vector3> points = new();

        Vector3 position = ctx.Origin;
        Vector3 velocity = ctx.Direction * ctx.Force;

        int bounces = 0;

        while (points.Count < ctx.MaxPoints)
        {
            velocity += ctx.GravityForce * ctx.Step;

            Vector3 next = position + velocity * ctx.Step;

            const uint collisionMask = 1 | 16;

            if (Utils.Raycast(ctx.Rat, position, next, out Dictionary hit, collisionMask))
            {
                position = hit["position"].AsVector3();
                points.Add(position);
                if ((GodotObject)hit["collider"] is Area3D area)
                {
                    FacilityBase facility = area.GetParent<FacilityBase>();
                    if (facility != null)
                    {
                        if (facility.TryGetThrowTarget(position, ctx.Rat, out ThrowTarget target))
                        {
                            bool isHoming = HomeTo(ctx, points, position, velocity, target.Position);
                            return new ThrowPath(points.ToArray(), target, homing: isHoming);
                        }
                    }
                }

                velocity = velocity.Bounce(hit["normal"].AsVector3()) * BounceDecay;

                if (++bounces > MaxBounces)
                    break;
            }
            else
            {
                position = next;
                points.Add(position);
            }
        }

        return new ThrowPath(points.ToArray());
    }
}