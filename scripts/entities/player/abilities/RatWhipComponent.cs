using System.Linq;
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
    public Vector3 AnchorPoint = Vector3.Zero;
    public Vector3 AnchorNormal = Vector3.Zero;
    public float RestLength;
    public bool IsAnchored;
    private ImmediateMesh _ratTailMesh = new();
    private bool _canAnchor;

    public override void _Ready()
    {
        RatTailMeshInstance.Mesh = _ratTailMesh;
        RatTailMeshInstance.MaterialOverride = new OrmMaterial3D() { AlbedoColor = Colors.Red };
    }

    public override void _Process(double delta)
    {
        _canAnchor = TryGetTargetAnchorPoint();
        TryGenerateMesh();
        if (_canAnchor)
            DebugDraw.Sphere(Player, AnchorPoint, .1f);
    }

    public void Release()
    {
        IsAnchored = false;
        AnchorPoint = Vector3.Zero;
    }

    public void EngageWhip()
    {
        IsAnchored = true;
        Player.ChangeMovementState<PlayerSwingState>();
        RestLength =
            Player.GlobalPosition.DistanceTo(AnchorPoint) * Player.Tuning.WhipRestLengthMultiplier;
    }

    public void LaunchToAnchor()
    {
        Vector3 target = AnchorPoint;
        float arcHeight = AnchorPoint.Y - Player.GlobalPosition.Y;

        PlayerArcMovementState state = Player.GetState<PlayerArcMovementState>();
        state.Configure(target, arcHeight);

        Player.ChangeMovementState<PlayerArcMovementState>();
        Release();
    }

    private float _anchorCheckHysteresis = 0.1f;

    private bool TryGetTargetAnchorPoint()
    {
        if (IsAnchored)
            return true;

        Vector3 startPoint = Player.Camera.GlobalPosition;
        Vector3 endPoint = startPoint - Player.Camera.GlobalBasis.Z * Player.Tuning.WhipMaxDistance;

        if (
            !RaycastUtils.Ray(
                Player,
                startPoint,
                endPoint,
                out Dictionary result,
                PhysicsLayers.WORLD
            )
        )
        {
            return false;
        }

        Vector3 hitPosition = result["position"].AsVector3();
        Vector3 normal = result["normal"].AsVector3();

        Vector3[] testPoints = RaycastUtils.FindCardinalEdges(Player, hitPosition, normal, 2f);

        if (testPoints.Length == 0)
            AnchorPoint = hitPosition;
        else
        {
            float minDistance = AnchorPoint.DistanceTo(hitPosition);
            Vector3 closest = AnchorPoint;
            foreach (Vector3 point in testPoints)
            {
                float distance = point.DistanceTo(hitPosition);
                if (distance < minDistance + _anchorCheckHysteresis)
                {
                    minDistance = distance;
                    closest = point;
                }
            }
            AnchorPoint = closest;
        }
        return true;
    }

    private void TryGenerateMesh()
    {
        _ratTailMesh.ClearSurfaces();
        if (!IsAnchored)
            return;

        _ratTailMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
        _ratTailMesh.SurfaceAddVertex(HandNode.GlobalPosition);
        _ratTailMesh.SurfaceAddVertex(AnchorPoint);
        _ratTailMesh.SurfaceEnd();
    }
}
