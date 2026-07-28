using Godot;
using Godot.Collections;
using System.Collections.Generic;


[GlobalClass]
public partial class SimpleThrow : ThrowType
{
    /// <summary>World geometry and facility bodies - these deflect the arc.</summary>
    private const uint SOLID_LAYER = 1;

    /// <summary>Facility catch volumes - detection only, they never deflect the arc.</summary>
    private const uint FACILITY_TRIGGER_LAYER = 16;

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

            if (!Utils.Raycast(ctx.Rat, position, next, out Dictionary hit, SOLID_LAYER | FACILITY_TRIGGER_LAYER))
            {
                position = next;
                points.Add(position);
                continue;
            }

            Vector3 hitPosition = hit["position"].AsVector3();

            if (hit["collider"].As<GodotObject>() is Area3D area)
            {
                if (area.GetParent() is FacilityBase facility
                    && facility.TryGetThrowTarget(hitPosition, ctx.Rat, out ThrowTarget target))
                {
                    points.Add(hitPosition);
                    bool isHoming = HomeTo(
                        ctx,
                        points,
                        hitPosition,
                        velocity,
                        target.Position,
                        ApproachClearance(ctx, hitPosition, target)
                    );
                    return new ThrowPath(points.ToArray(), target, homing: isHoming);
                }

                // Nothing to aim at here, so pass through the trigger and let the facility's own
                // body produce the bounce instead of the catch volume.
                if (!Utils.Raycast(ctx.Rat, hitPosition, next, out hit, SOLID_LAYER, collideWithAreas: false))
                {
                    position = next;
                    points.Add(position);
                    continue;
                }

                hitPosition = hit["position"].AsVector3();
            }

            position = hitPosition;
            points.Add(position);

            velocity = velocity.Bounce(hit["normal"].AsVector3()) * BounceDecay;

            if (++bounces > MaxBounces)
                break;
        }

        return new ThrowPath(points.ToArray());
    }

    /// <summary>
    /// Only arc over the facility when the direct approach is actually blocked, so near-side slots
    /// keep their flat, readable curve.
    /// </summary>
    private float ApproachClearance(ThrowContext ctx, Vector3 from, ThrowTarget target)
    {
        Vector3 approach = target.Position + Vector3.Up * ApproachHeight;

        return Utils.Raycast(ctx.Rat, from, approach, out _, SOLID_LAYER, collideWithAreas: false)
            ? target.Facility.ColliderTopY
            : float.NegativeInfinity;
    }
}
