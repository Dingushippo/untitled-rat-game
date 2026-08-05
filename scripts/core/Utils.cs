using Godot;
using Godot.Collections;
using System;
using System.Linq;

public static partial class Utils
{
    public static bool Raycast(
        Node3D node,
        Vector3 a,
        Vector3 b,
        out Dictionary result,
        uint collisionMask = 4294967295,
        bool collideWithAreas = true,
        bool collideWithBodies = true,
        Func<GodotObject, bool> accept = null,
        int maxDepth = 5
    )
    {
        if (accept == null)
        {
            result = RaycastHelper(node, a, b, collisionMask, collideWithAreas, collideWithBodies);
            return result.Count != 0;
        }
        Array<Rid> excludeRidArray = [];
        int depth = 0;
        result = [];
        while (depth < maxDepth)
        {
            result = RaycastHelper(node, a, b, collisionMask, collideWithAreas, collideWithBodies, excludeRidArray);
            if (result.Count > 0 && !accept(result["collider"].As<GodotObject>()))
            {
                excludeRidArray.Add(result["rid"].As<Rid>());
                depth++;
                continue;
            }
            return result.Count != 0;
        }
        result = [];
        return false;
    }

    private static Dictionary RaycastHelper(
        Node3D node,
        Vector3 a,
        Vector3 b,
        uint collisionMask = 4294967295,
        bool collideWithAreas = true,
        bool collideWithBodies = true,
        Array<Rid> excludeRidArray = null
    )
    {
        PhysicsDirectSpaceState3D state = node.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            a, b, collisionMask
        );
        query.CollideWithAreas = collideWithAreas;
        query.CollideWithBodies = collideWithBodies;
        query.Exclude = excludeRidArray;
        Profiler.Count("physics.raycast");
        return state.IntersectRay(query);
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
        return result.Count != 0;
    }
}