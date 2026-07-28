using Godot;
using Godot.Collections;


[GlobalClass]
public partial class SimpleThrow : ThrowType
{
    /// <summary>World geometry and facility bodies - these deflect the arc.</summary>
    private const uint SOLID_LAYER = 1;

    /// <summary>Facility catch volumes - detection only, they never deflect the arc.</summary>
    private const uint FACILITY_TRIGGER_LAYER = 16;

    [ExportGroup("Bounce")]
    /// <summary>Fraction of the into-surface speed that comes back out. A rat is not a superball.</summary>
    [Export(PropertyHint.Range, "0,1,0.01")] public float Restitution = 0.35f;

    /// <summary>Fraction of the along-surface speed scrubbed off by the impact.</summary>
    [Export(PropertyHint.Range, "0,1,0.01")] public float SurfaceFriction = 0.3f;

    /// <summary>Below this the rebound is too feeble to be worth drawing; drop and let it land.</summary>
    [Export(PropertyHint.Range, "0,10,0.1")] public float MinBounceSpeed = 1.5f;

    [Export] public int MaxBounces = 1;

    public override ThrowPath Simulate(ThrowContext ctx)
    {
        ThrowPathBuilder path = new();

        Vector3 position = ctx.Origin;
        Vector3 velocity = ctx.Direction * ctx.Force;

        int bounces = 0;

        while (path.Count < ctx.MaxPoints)
        {
            velocity += ctx.GravityForce * ctx.Step;

            Vector3 next = position + velocity * ctx.Step;

            if (!Utils.Raycast(ctx.Rat, position, next, out Dictionary hit, SOLID_LAYER | FACILITY_TRIGGER_LAYER))
            {
                position = next;
                path.Add(position, velocity.Length());
                continue;
            }

            Vector3 hitPosition = hit["position"].AsVector3();

            if (hit["collider"].As<GodotObject>() is Area3D area)
            {
                if (area.GetParent() is FacilityBase facility
                    && facility.TryGetThrowTarget(hitPosition, ctx.Rat, out ThrowTarget target))
                {
                    path.Add(hitPosition, velocity.Length());
                    bool isHoming = HomeTo(
                        ctx,
                        path,
                        hitPosition,
                        velocity,
                        target.Position,
                        ApproachClearance(ctx, hitPosition, target)
                    );
                    return path.Build(target, homing: isHoming);
                }

                // Nothing to aim at here, so pass through the trigger and let the facility's own
                // body produce the bounce instead of the catch volume.
                if (!Utils.Raycast(ctx.Rat, hitPosition, next, out hit, SOLID_LAYER, collideWithAreas: false))
                {
                    position = next;
                    path.Add(position, velocity.Length());
                    continue;
                }

                hitPosition = hit["position"].AsVector3();
            }

            position = hitPosition;
            path.Add(position, velocity.Length());

            velocity = Deflect(velocity, hit["normal"].AsVector3());

            if (++bounces > MaxBounces || velocity.Length() < MinBounceSpeed)
                break;
        }

        return path.Build();
    }

    /// <summary>
    /// Splits the impact into its into-surface and along-surface parts and damps them separately.
    /// A single uniform decay on <c>Vector3.Bounce</c> cannot express that: it returns the same
    /// fraction of speed no matter how the rat hits, so a near-vertical throw rebounded almost as
    /// fast as it arrived. Restitution is what kills that, friction is what makes glancing hits
    /// skid instead of skipping.
    /// </summary>
    private Vector3 Deflect(Vector3 velocity, Vector3 normal)
    {
        Vector3 intoSurface = normal * velocity.Dot(normal);
        Vector3 alongSurface = velocity - intoSurface;

        return alongSurface * (1f - SurfaceFriction) - intoSurface * Restitution;
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
