using Godot;

public readonly struct ThrowContext
{
    public readonly Rat Rat;
    public readonly Vector3 Origin;
    public readonly Vector3 Direction;
    public readonly float Force;
    public readonly Vector3 Gravity;
    public readonly float AscentGravityScale;
    public readonly float DescentGravityScale;
    public readonly float DescentBlendSpeed;
    public readonly float Step;
    public readonly int MaxPoints;

    public ThrowContext(
        Rat rat,
        Vector3 origin,
        Vector3 direction,
        float force,
        Vector3 gravity,
        float ascent,
        float descent,
        float descentBlendSpeed,
        float step,
        int maxPoints
    )
    {
        Rat = rat;
        Origin = origin;
        Direction = direction;
        Force = force;
        Gravity = gravity;
        AscentGravityScale = ascent;
        DescentGravityScale = descent;
        DescentBlendSpeed = descentBlendSpeed;
        Step = step;
        MaxPoints = maxPoints;
    }
}