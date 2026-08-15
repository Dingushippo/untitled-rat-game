using Godot;


[GlobalClass]
public abstract partial class ThrowType : Resource
{
    private const int BEZIER_RESOLUTION = 64;

    [Export] public float SteeringStrength = 8f;
    [Export] public float ArrivalDistance = 0.05f;
    [Export] public float ApproachHeight = 0.5f;
    [Export] public float CurveSpacing = 0.05f;
    [Export(PropertyHint.Range, "0, 1.0")] public float TangentStrength = 0.2f;

    /// <summary>Speed the rat eases down to as the homing curve delivers it, so arrival isn't a stop-dead.</summary>
    [Export(PropertyHint.Range, "0.5,20,0.1")] public float ArrivalSpeed = 5f;

    public abstract ThrowPath Simulate(ThrowContext ctx);

    protected Vector3 Steer(Vector3 pos, Vector3 vel, Vector3 target, float step)
    {
        Vector3 desired = (target - pos).Normalized() * vel.Length();
        return vel.Lerp(desired, Mathf.Clamp(SteeringStrength * step, 0, 1));
    }

    protected bool HomeTo(ThrowContext ctx, ThrowPathBuilder path, Vector3 pos, Vector3 vel, Vector3 target, float clearanceY = float.NegativeInfinity)
    {
        float entrySpeed = vel.Length();

        Vector3 p0 = pos;
        Vector3 p1 = p0 + vel * pos.DistanceTo(target) * TangentStrength;
        p1.Y = Mathf.Max(p1.Y, target.Y);
        Vector3 p2 = target + Vector3.Up * ApproachHeight;
        Vector3 p3 = target;

        // Lift the middle control points over the structure, otherwise a curve aimed at a slot on
        // the far side cuts straight through the facility's collider.
        if (!float.IsNegativeInfinity(clearanceY))
        {
            float minY = clearanceY + ApproachHeight;
            p1.Y = Mathf.Max(p1.Y, minY);
            p2.Y = Mathf.Max(p2.Y, minY);
        }

        // Sample finely first so the curve can be re-sampled by arc length -
        // stepping t uniformly bunches points up wherever the curve is tight.
        Vector3[] samples = new Vector3[BEZIER_RESOLUTION + 1];
        float[] travelled = new float[BEZIER_RESOLUTION + 1];

        path.CurrentSegment++;

        for (int i = 0; i <= BEZIER_RESOLUTION; i++)
        {
            samples[i] = p0.BezierInterpolate(p1, p2, p3, (float)i / BEZIER_RESOLUTION);
            if (i > 0)
            {
                travelled[i] = travelled[i - 1] + samples[i].DistanceTo(samples[i - 1]);
            }
        }

        float length = travelled[BEZIER_RESOLUTION];
        if (length < 0.0001f)
        {
            path.Add(target, ArrivalSpeed);
            return true;
        }

        int count = Mathf.Max(2, Mathf.CeilToInt(length / Mathf.Max(CurveSpacing, 0.01f)));
        int segment = 1;

        for (int i = 1; i <= count; i++)
        {
            float distance = length * i / count;

            while (segment < BEZIER_RESOLUTION && travelled[segment] < distance)
            {
                segment++;
            }

            float segmentLength = travelled[segment] - travelled[segment - 1];
            float weight = segmentLength > 0.0001f
                ? (distance - travelled[segment - 1]) / segmentLength
                : 0f;

            path.Add(
                samples[segment - 1].Lerp(samples[segment], weight),
                Mathf.Lerp(entrySpeed, ArrivalSpeed, (float)i / count)
            );
        }

        return true;
    }
}