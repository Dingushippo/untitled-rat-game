using Godot;
using Godot.Collections;


[GlobalClass]
public partial class SimpleThrow : ThrowType
{
    const float MIN_STEP_FRACTION = 0.1f;
    [Export] public float CollisionRadius = 0.25f;

    [ExportGroup("Bounce")]
    /// <summary>Fraction of the into-surface speed that comes back out. A rat is not a superball.</summary>
    [Export(PropertyHint.Range, "0,1,0.01")] public float Restitution = 0.35f;

    /// <summary>Fraction of the along-surface speed scrubbed off by the impact.</summary>
    [Export(PropertyHint.Range, "0,1,0.01")] public float SurfaceFriction = 0.3f;

    /// <summary>Below this the rebound is too feeble to be worth drawing; drop and let it land.</summary>
    [Export(PropertyHint.Range, "0,10,0.1")] public float MinBounceSpeed = 1.5f;

    [Export] public int MaxBounces = 1;


    private float GravityScale(ThrowContext ctx, Vector3 velocity, int bounces)
    {
        if (bounces > 0) return ctx.DescentGravityScale;

        float fall = Mathf.SmoothStep(0f, ctx.DescentBlendSpeed, -velocity.Y);
        return Mathf.Lerp(ctx.AscentGravityScale, ctx.DescentGravityScale, fall);
    }

    private Vector3 Retreat(Vector3 hitPosition, Vector3 direction, Vector3 normal, Vector3 previous)
    {
        float approach = Mathf.Max(-direction.Dot(normal), 0.2f);
        float backoff = Mathf.Min(CollisionRadius / approach, CollisionRadius * 1.5f);

        // Prevent path from doubling back on itself
        backoff = Mathf.Min(backoff, previous.DistanceTo(hitPosition));

        return hitPosition - direction * backoff;
    }
    public override ThrowPath Simulate(ThrowContext ctx)
    {
        ThrowPathBuilder path = new();
        uint collisionMask = PhysicsLayers.GetOrMask(PhysicsLayers.WORLD, PhysicsLayers.FACILITY, PhysicsLayers.CATCH_AREA);

        Vector3 position = ctx.Origin;
        Vector3 velocity = ctx.Direction * ctx.Force;
        // Vector3 handOffset = 

        int bounces = 0;
        float carry = 0f;
        bool settled = false;

        while (path.Count < ctx.MaxPoints && !settled)
        {
            float step = carry > ctx.Step * MIN_STEP_FRACTION ? carry : ctx.Step;
            carry = 0f;

            velocity += ctx.Gravity * GravityScale(ctx, velocity, bounces) * step;
            Vector3 next = position + velocity * step;

            if (!Utils.Raycast(ctx.Rat, position, next, out Dictionary hit, collisionMask))
            {
                position = next;
                path.Add(position, velocity.Length());
                continue;
            }

            Vector3 hitPosition = hit["position"].AsVector3();

            if (hit["collider"].As<GodotObject>() is Area3D area)
            {
                if (area.GetParent() is ICatchArea facility && facility.TryGetThrowTarget(hitPosition, ctx.Rat, out ThrowTarget target))
                {
                    path.Add(hitPosition, velocity.Length());
                    bool isHoming = HomeTo(ctx, path, hitPosition, velocity, target.Position, ApproachClearance(ctx, hitPosition, target));
                    return path.Build(target, homing: isHoming);
                }
                // Nothing to aim at here, so pass through the trigger and let the facility's own body produce the bounce instead of the catch volume
                if (!Utils.Raycast(ctx.Rat, hitPosition, next, out hit, PhysicsLayers.WORLD, collideWithAreas: false))
                {
                    position = next;
                    path.Add(position, velocity.Length());
                    continue;
                }

                hitPosition = hit["position"].AsVector3();
            }

            Vector3 normal = hit["normal"].AsVector3();
            Vector3 direction = velocity.Normalized();

            // Only part of this step was actually used, the rest carries into the rebound so the bounce isn't given a free full step of gravity and travel
            float full = position.DistanceTo(next);
            if (full > 0.0001f) // Arbitrarily small number
                carry = step * (1f - position.DistanceTo(hitPosition) / full);

            position = Retreat(hitPosition, direction, normal, position);

            // Only the into-surface component decides whether a bounce is worth having.
            // Testing the full impact speed instead lets a fast horizontal skid bounce forever along a flat floor
            float impactSpeed = -velocity.Dot(normal);
            velocity = Deflect(velocity, normal);
            path.Add(position, velocity.Length());
            bounces++;
            path.CurrentSegment++;
            path.AddImpact();

            if (impactSpeed < MinBounceSpeed)
            {
                settled = true;
                path.ExitVelocity = Vector3.Zero;
            }
            else if (bounces > MaxBounces)
            {
                settled = true;
            }
        }

        if (!settled)
            path.ExitVelocity = velocity;

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

        return Utils.Raycast(ctx.Rat, from, approach, out _, PhysicsLayers.WORLD, collideWithAreas: false)
            ? target.ColliderTopY
            : float.NegativeInfinity;
    }
}
