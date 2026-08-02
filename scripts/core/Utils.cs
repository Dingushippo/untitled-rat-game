using Godot;
using Godot.Collections;
using System.Collections;
using System.Runtime.InteropServices;

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

    public static bool ShapeCast(
        Node3D node,
        CollisionShape3D collider,
        out Array<Dictionary> result,
        uint collisionMask = 4294967295,
        bool collideWithAreas = true,
        bool collideWithBodies = true
    )
    {
        PhysicsDirectSpaceState3D state = node.GetWorld3D().DirectSpaceState;
        PhysicsShapeQueryParameters3D query = new()
        {
            Shape = collider.Shape,
            CollisionMask = collisionMask,
            CollideWithAreas = collideWithAreas,
            CollideWithBodies = collideWithBodies
        };

        result = state.IntersectShape(query);
        GD.Print(result);
        return result.Count != 0;
    }
}