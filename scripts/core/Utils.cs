using Godot;
using Godot.Collections;

public static partial class Utils
{
    public static bool Raycast(
        Node3D node,
        Vector3 a,
        Vector3 b,
        out Dictionary result,
        uint collisionMask = 4294967295,
        bool collideWithAreas = true,
        bool collideWithBodies = true)
    {
        PhysicsDirectSpaceState3D state = node.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            a, b, collisionMask
        );
        query.CollideWithAreas = collideWithAreas;
        query.CollideWithBodies = collideWithBodies;
        result = state.IntersectRay(query);

        Profiler.Count("physics.raycast");

        return result.Count != 0;
    }
}