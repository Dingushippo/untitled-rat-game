using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RatWhipComponent : MovementAbility
{
    [Export] public float MaxWhipDistance = 15f;
    [Export] public float WhipRetraction = 5f;
    [Export] public float WhipSwingForce = 3f;
    [Export] public float WhipRestLengthMultiplier = 1f;
    public MeshInstance3D RatTailMeshInstance;
    public Vector3 AnchorPoint = Vector3.Zero;
    public Vector3 AnchorNormal = Vector3.Zero;
    public float RestLength;
    public bool IsAnchored;
    private ImmediateMesh _ratTailMesh = new();
    private bool _canAnchor;
    private bool _anchorIsOnTopLedge;

    private float _anchorCheckHysteresis = 0.1f;
    private Vector3 _hitPosition;
    private Vector3 _hitNormal;
    private Node3D _handNode;


    public override void Init(Player player, HierarchicalStateMachine hfsm)
    {
        base.Init(player, hfsm);
        _handNode = _player.HandL;
        RatTailMeshInstance = new();
        _handNode.AddChild(RatTailMeshInstance);
        RatTailMeshInstance.Mesh = _ratTailMesh;
    }
    public override void OnActivate()
    {
        IsAnchored = true;
        RestLength = _player.GlobalPosition.DistanceTo(AnchorPoint) * WhipRestLengthMultiplier;
    }

    public override void OnDeactivate()
    {
        IsAnchored = false;
        AnchorPoint = Vector3.Zero;
    }

    private bool TryGetTargetAnchorPoint()
    {
        if (IsAnchored)
            return true;

        Vector3 startPoint = _player.Camera.GlobalPosition;
        Vector3 endPoint = startPoint - _player.Camera.GlobalBasis.Z * _player.Tuning.WhipMaxDistance;

        if (
            !RaycastUtils.Ray(
                _player,
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

        Vector3[] testPoints = RaycastUtils.FindCardinalEdges(_player, _hitPosition, _hitNormal, 2f);

        DebugDraw.Sphere(_player, _hitPosition, .1f, Colors.Orange);
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
            DebugDraw.Sphere(_player, AnchorPoint, .1f, color);
        }
        return true;
    }

    private void TryGenerateMesh()
    {
        _ratTailMesh.ClearSurfaces();
        if (!IsAnchored)
            return;

        _ratTailMesh.SurfaceBegin(Mesh.PrimitiveType.LineStrip);
        _ratTailMesh.SurfaceAddVertex(_handNode.GlobalPosition);
        _ratTailMesh.SurfaceAddVertex(AnchorPoint);
        _ratTailMesh.SurfaceEnd();
    }

    public override void PhysicsProcess(float delta)
    {
        // 1. Check for state transition
        if (!_player.Input.LeftArmAction)
        {
            _hfsm.ChangeState<PlayerFallingState>();
            return;
        }

        Vector3 toAnchor = AnchorPoint - _player.GlobalPosition;
        float currentLength = toAnchor.Length();
        if (currentLength <= 0.001f) return;

        Vector3 ropeDirection = toAnchor.Normalized();

        // 2. Build our base velocities for this frame (Gravity + Swing Input)
        Vector3 nextVelocity = _player.Velocity;

        // Apply standard gravity vector to our velocity profile
        nextVelocity += _player.GetGravity() * delta;

        // Apply player's WASD swing acceleration along the swing plane
        Vector3 inputDir = _player.Input.Direction;
        if (inputDir.Length() > 0)
        {
            Plane swingPlane = new Plane(ropeDirection, 0);
            Vector3 tangentialInputDir = swingPlane.Project(inputDir).Normalized();
            nextVelocity += tangentialInputDir * WhipSwingForce * delta; // Scaled by delta for consistency
        }

        // 3. Constrain the movement geometrically (The CharacterBody approach)
        float stretch = currentLength - RestLength;

        if (stretch > 0)
        {
            // Find out how fast the player is moving away from or toward the anchor point
            float radialVelocity = nextVelocity.Dot(ropeDirection);

            if (radialVelocity < 0)
            {
                // The player is moving outwards, attempting to break/stretch the rope constraint.
                // We cancel out the outward velocity vector entirely, forcing them into a clean arc.
                nextVelocity -= ropeDirection * radialVelocity;
            }

            // Optional: Actively retract the rope if you want an auto-pull effect over time
            nextVelocity += ropeDirection * WhipRetraction * delta;
        }

        // 4. Assign velocity profile and let Godot run its collision resolution slide loop
        _player.Velocity = nextVelocity;
        _player.MoveAndSlide();

        // 5. Late correction safety net: Prevents CharacterBody from drifting outside rope bounds 
        // due to rounding errors during MoveAndSlide() collisions.
        Vector3 postMoveToAnchor = AnchorPoint - _player.GlobalPosition;
        if (postMoveToAnchor.Length() > RestLength)
        {
            Vector3 targetPosition = AnchorPoint - (postMoveToAnchor.Normalized() * RestLength);
            _player.GlobalPosition = _player.GlobalPosition.MoveToward(targetPosition, 10 * delta);
        }
    }

    public override void WhileEquipped()
    {
        _canAnchor = TryGetTargetAnchorPoint();
        TryGenerateMesh();
        DebugDraw.Sphere(_player, AnchorPoint, .1f, Colors.SkyBlue);

        if (!_anchorIsOnTopLedge)
        {
            Vector3 testDir = AnchorPoint.DirectionTo(_hitPosition);
            Vector3 newPoint = AnchorPoint - testDir * 1f;
            DebugDraw.Sphere(_player, newPoint, .1f, Colors.SkyBlue);
        }
    }
}
