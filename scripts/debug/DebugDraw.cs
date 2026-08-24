using System.Collections.Generic;
using Godot;

public static class DebugDraw
{
    private class DebugShape
    {
        public Rid InstanceRid;
        public Mesh Mesh;
        public Material Material;
        public bool Static = false;
    }

    private static readonly List<DebugShape> _activeShapes = new();
    private static readonly List<DebugShape> _pendingClear = new();

    public static void Sphere(
        Node3D node,
        Vector3 position,
        float radius = 0.5f,
        Color? color = null
    )
    {
        if (node == null || !node.IsInsideTree())
            return;

        World3D scenario = node.GetWorld3D();
        if (scenario == null)
            return;

        Color drawColor = color ?? Colors.Red;

        // Create Mesh & Unshaded Material with NoDepthTest so it shows through geometry
        SphereMesh mesh = new() { Radius = radius, Height = radius * 2f };
        StandardMaterial3D material = new()
        {
            AlbedoColor = drawColor,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            NoDepthTest = true, // Ensures debug shape isn't hidden inside objects
        };

        Rid instanceRid = RenderingServer.InstanceCreate();
        RenderingServer.InstanceSetBase(instanceRid, mesh.GetRid());
        RenderingServer.InstanceSetScenario(instanceRid, scenario.Scenario);
        RenderingServer.InstanceSetTransform(
            instanceRid,
            Transform3D.Identity.Translated(position)
        );
        RenderingServer.InstanceGeometrySetMaterialOverride(instanceRid, material.GetRid());

        // Explicitly enable visibility and default layer mask
        RenderingServer.InstanceSetVisible(instanceRid, true);
        RenderingServer.InstanceSetLayerMask(instanceRid, 1);

        _activeShapes.Add(
            new DebugShape
            {
                InstanceRid = instanceRid,
                Mesh = mesh,
                Material = material,
            }
        );
    }

    /// <summary>
    /// Call this at the START of your main controller's _Process loop (NOT at the end).
    /// </summary>
    public static void Clear()
    {
        // Free frame N-1 shapes now that the previous frame finished rendering
        foreach (var shape in _activeShapes)
        {
            if (shape.InstanceRid.IsValid)
            {
                RenderingServer.FreeRid(shape.InstanceRid);
            }
        }
        _activeShapes.Clear();
    }
}
