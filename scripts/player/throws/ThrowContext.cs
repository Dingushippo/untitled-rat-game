using Godot;

public readonly struct ThrowContext
{
    public readonly Node3D Source;
    public readonly Vector3 Origin;
    public readonly Vector3 Direction;
    public readonly float Force;
    public readonly Vector3 Gravity;
    public readonly float Step;
    public readonly int MaxPoints;

    public ThrowContext(Node3D source, Vector3 origin, Vector3 direction, float force, Vector3 gravity, float step, int maxPoints)
    {
        Source = source;
        Origin = origin;
        Direction = direction;
        Force = force;
        Gravity = gravity;
        Step = step;
        MaxPoints = maxPoints;
    }
}