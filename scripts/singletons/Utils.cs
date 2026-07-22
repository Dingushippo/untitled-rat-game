using Godot;
using Godot.Collections;

public static partial class Utils
{
    public static bool Raycast(Node3D node, Vector3 a, Vector3 b, out Dictionary result, uint collisionMask = 4294967295)
    {
        PhysicsDirectSpaceState3D state = node.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            a, b, collisionMask
        );
        result = state.IntersectRay(query);
        return result.Count != 0;
    }
}