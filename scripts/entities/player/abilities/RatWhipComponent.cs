using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RatWhipComponent : Node
{
    public const float WHIP_RETRACTION = 5f;

    [Export]
    public Player Player;

    [Export]
    public Node3D HandNode;

    [Export]
    public MeshInstance3D RatTailMeshInstance;

    public Generic6DofJoint3D Joint;
    public float WhipLength => Player.HandL.GlobalPosition.DistanceTo(AnchorPoint);
    public Vector3 AnchorPoint = Vector3.Zero;
    public bool IsAnchored => AnchorPoint != Vector3.Zero;
    private ImmediateMesh _ratTailMesh = new();

    public override void _Ready()
    {
        RatTailMeshInstance.Mesh = _ratTailMesh;
        RatTailMeshInstance.MaterialOverride = new OrmMaterial3D() { AlbedoColor = Colors.Red };
    }

    public override void _Process(double delta)
    {
        TryGenerateMesh();
    }

    public void Release()
    {
        AnchorPoint = Vector3.Zero;
        AnchorObject = null;
        // Player.ChangeMovementState<PlayerFallingState>();
    }

    public void EngageWhip(float maxDistance)
    {
        if (!TryGetTargetAnchorPoint(maxDistance))
            return;

        Player.ChangeMovementState<PlayerSwingState>();
    }

    public StaticBody3D AnchorObject;

    private bool TryGetTargetAnchorPoint(float maxDistance)
    {
        Vector3 startPoint = Player.Camera.GlobalPosition;
        Vector3 endPoint = startPoint - Player.Camera.GlobalBasis.Z * maxDistance;
        if (
            RaycastUtils.Ray(
                Player,
                startPoint,
                endPoint,
                out Dictionary result,
                PhysicsLayers.WORLD
            )
        )
        {
            AnchorPoint = result["position"].AsVector3();
            AnchorObject = result["collider"].As<StaticBody3D>();
            return true;
        }
        return false;
    }

    private void TryGenerateMesh()
    {
        _ratTailMesh.ClearSurfaces();
        if (AnchorPoint == Vector3.Zero)
            return;

        _ratTailMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
        _ratTailMesh.SurfaceAddVertex(HandNode.GlobalPosition);
        _ratTailMesh.SurfaceAddVertex(AnchorPoint);
        _ratTailMesh.SurfaceEnd();
    }
}
