using System.Collections.Generic;
using Godot;
using Godot.Collections;

public sealed class RatWhipComponent
{
    private const float RAT_LENGTH = 0.7f;
    private const float MIN_TAIL_EXTENT = 0.2f;
    private Player _player;
    private Node3D _startNode;
    private Node _scene;
    private List<Rat> _rats = new();
    private List<ConeTwistJoint3D> _joints = new();

    public RatWhipComponent(Player player)
    {
        _player = player;
        _startNode = player.HandL;
        _scene = player.GetTree().CurrentScene;
    }

    private void ReleaseWhip()
    {
        if (_rats.Count == 0)
            return;
        RatManager.Instance.Despawn(_rats);
        _joints.ForEach(j => j.QueueFree());
        _rats.Clear();
        _joints.Clear();
    }

    public void SpawnWhipNodes(float maxDistance)
    {
        ReleaseWhip();
        if (!TryGetTargetAnchorPoint(out Vector3 anchorPosition, out Node3D collider, maxDistance))
            return;

        Vector3[] points = GetWhipNodePositions(anchorPosition);

        if (!RatManager.Instance.CanSpawnRats(points.Length))
            return;

        for (int i = 0; i < points.Length; i++)
        {
            RatManager.Instance.TrySpawnRat(out Rat rat, points[i]);
            rat.LookAt(anchorPosition);
            _rats.Add(rat);

            rat.Freeze = false;
            rat.Collider.Disabled = false;

            if (i == 0)
                continue;

            ConeTwistJoint3D joint = new()
            {
                NodeA = _rats[i - 1].GetPath(),
                NodeB = _rats[i].GetPath(),
            };
            _rats[i - 1].AddChild(joint);
            joint.GlobalPosition = _rats[i - 1].GlobalPosition.Lerp(_rats[i].GlobalPosition, 0.5f);
            _joints.Add(joint);
        }

        _rats[0].Freeze = _rats[^1].Freeze = true;
    }

    private bool TryGetTargetAnchorPoint(
        out Vector3 position,
        out Node3D collider,
        float maxDistance
    )
    {
        Vector3 startPoint = _player.Camera.GlobalPosition;
        Vector3 endPoint = startPoint - _player.Camera.GlobalBasis.Z * maxDistance;
        position = default;
        collider = default;
        if (
            RaycastUtils.Ray(
                _player,
                startPoint,
                endPoint,
                out Dictionary result,
                PhysicsLayers.WORLD
            )
        )
        {
            collider = result["collider"].As<Node3D>();
            position = result["position"].AsVector3();
            return true;
        }
        return false;
    }

    private Vector3[] GetWhipNodePositions(Vector3 anchorPos)
    {
        Vector3 startPos = _startNode.GlobalPosition;
        float whipLength = startPos.DistanceTo(anchorPos);
        int numSegments = Mathf.FloorToInt(whipLength / (RAT_LENGTH + MIN_TAIL_EXTENT));
        List<Vector3> points = new();
        for (int i = 0; i < numSegments; i++)
        {
            float t = i / (float)numSegments;
            Vector3 lerped = startPos.Lerp(anchorPos, t);
            points.Add(lerped);
        }
        return points.ToArray();
    }
}
