using System;
using Godot;
using Godot.Collections;

public static class RaycastUtils
{
    public static bool Ray(
        Node3D node,
        Vector3 a,
        Vector3 b,
        out Dictionary result,
        uint collisionMask = uint.MaxValue,
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
            result = RaycastHelper(
                node,
                a,
                b,
                collisionMask,
                collideWithAreas,
                collideWithBodies,
                excludeRidArray
            );
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
        uint collisionMask = uint.MaxValue,
        bool collideWithAreas = true,
        bool collideWithBodies = true,
        Array<Rid> excludeRidArray = null
    )
    {
        PhysicsDirectSpaceState3D state = node.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(a, b, collisionMask);
        query.CollideWithAreas = collideWithAreas;
        query.CollideWithBodies = collideWithBodies;
        query.HitFromInside = true;
        query.Exclude = excludeRidArray;
        Profiler.Count("physics.raycast");
        return state.IntersectRay(query);
    }

    public static bool Shape(
        Node3D node,
        CollisionShape3D collider,
        out Array<Dictionary> result,
        uint collisionMask = uint.MaxValue,
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
            CollideWithBodies = collideWithBodies,
        };

        result = state.IntersectShape(query);
        return result.Count != 0;
    }

    public static bool CircleShape(
        Node3D node,
        Vector3 position,
        float radius,
        out Array<Dictionary> result,
        uint collisionMask = uint.MaxValue,
        bool collideWithAreas = true,
        bool collideWithBodies = true
    )
    {
        PhysicsDirectSpaceState3D state = node.GetWorld3D().DirectSpaceState;

        Rid shapeRid = PhysicsServer3D.CylinderShapeCreate();
        Dictionary<string, float> shapeData = new();
        shapeData["height"] = 0.2f;
        shapeData["radius"] = radius;
        PhysicsServer3D.ShapeSetData(shapeRid, shapeData);

        PhysicsShapeQueryParameters3D query = new()
        {
            ShapeRid = shapeRid,
            CollisionMask = collisionMask,
            CollideWithAreas = collideWithAreas,
            CollideWithBodies = collideWithBodies,
            Transform = new Transform3D(Basis.Identity, position + Vector3.Up * 0.4f),
        };

        result = state.IntersectShape(query);
        PhysicsServer3D.FreeRid(shapeRid);
        return result.Count != 0;
    }

    public static bool Circle(
        Node3D node,
        Vector3 position,
        float radius,
        int samples,
        out Dictionary result,
        uint collisionMask = uint.MaxValue,
        bool collideWithAreas = true,
        bool collideWithBodies = true,
        Func<GodotObject, bool> accept = null,
        int maxDepth = 5
    )
    {
        result = default;
        for (int i = 0; i < samples; i++)
        {
            float angle = i * (Mathf.Tau / samples);
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 endPos = position + direction * radius;
            if (
                Ray(
                    node,
                    position,
                    endPos,
                    out result,
                    collisionMask,
                    collideWithAreas,
                    collideWithBodies,
                    accept,
                    maxDepth
                )
            )
                return true;
        }
        return false;
    }
}
