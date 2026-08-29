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
    private bool _anchorIsOnTopLedge;

    public override void _Ready()
    {
        RatTailMeshInstance.Mesh = _ratTailMesh;
        RatTailMeshInstance.MaterialOverride = new OrmMaterial3D() { AlbedoColor = Colors.Red };
    }

    public override void _Process(double delta)
    {
        _canAnchor = TryGetTargetAnchorPoint();
        TryGenerateMesh();

        if (!_anchorIsOnTopLedge)
        {
            Vector3 testDir = AnchorPoint.DirectionTo(_hitPosition);
            Vector3 newPoint = AnchorPoint - testDir * 1f;
            DebugDraw.Sphere(Player, newPoint, .1f, Colors.SkyBlue);
        }
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

        if (_anchorIsOnTopLedge)
        {
            PlayerArcMovementState state = Player.GetState<PlayerArcMovementState>();
            state.Configure(target, arcHeight);

            Player.ChangeMovementState<PlayerArcMovementState>();
        }
        else
        {
            Vector3 testDir = AnchorPoint.DirectionTo(_hitPosition);
            Vector3 launchTowardsPoint = AnchorPoint - testDir * 1f;
            Vector3 launchVector = Player.GlobalPosition.DirectionTo(launchTowardsPoint);

            PlayerJumpState state = Player.GetState<PlayerJumpState>();
            state.Configure(launchVector, 300f);

            Player.ChangeMovementState<PlayerJumpState>();
        }
        Release();
    }

    private float _anchorCheckHysteresis = 0.1f;
    private Vector3 _hitPosition;
    private Vector3 _hitNormal;

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

        _hitPosition = result["position"].AsVector3();
        _hitNormal = result["normal"].AsVector3();

        Vector3[] testPoints = RaycastUtils.FindCardinalEdges(Player, _hitPosition, _hitNormal, 2f);

        DebugDraw.Sphere(Player, _hitPosition, .1f, Colors.Orange);
        bool isTopSurface = _hitNormal == Vector3.Up;
        if (testPoints.Length == 0 || isTopSurface)
        {
            AnchorPoint = _hitPosition;
            _anchorIsOnTopLedge = isTopSurface;
        }
        else
        {
            float minDistance = AnchorPoint.DistanceTo(_hitPosition);
            Vector3 closest = AnchorPoint;
            foreach (Vector3 point in testPoints)
            {
                float distance = point.DistanceTo(_hitPosition);
                if (distance < minDistance + _anchorCheckHysteresis)
                {
                    minDistance = distance;
                    closest = point;
                }
            }
            AnchorPoint = closest;
            _anchorIsOnTopLedge = AnchorPoint.Y > _hitPosition.Y || _hitNormal == Vector3.Up;

            Color color = _anchorIsOnTopLedge ? Colors.Green : Colors.Red;
            DebugDraw.Sphere(Player, AnchorPoint, .1f, color);
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
