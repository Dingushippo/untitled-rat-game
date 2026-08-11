using Godot;
using Godot.Collections;
using System.Linq;

public partial class RitualComponent
{
    private const float MAX_DISTANCE = 30f;
    private const float VALID_DISTANCE = 5f;
    private const float COLLISION_CHECK_MARGIN = .5f;
    public bool ValidPosition;

    private Array<RitualResource> _resources = []; // Should be preloaded on start
    private RitualBase _currentRitual;
    private Player _player;
    private PlayerCamera _camera => _player.Camera;
    private float _maxRadiusToCheckCollision;
    private Color _previewColor = Colors.Aquamarine;
    private Color _errorColor = Colors.Red;
    private Color _placedColor = Colors.White;
    private Color _prevColor;
    private uint _collisionMask;
    private IRitualInteract ritualInteract;
    private bool _initialSnap = true;


    public RitualComponent(Player player)
    {
        _player = player;
        _collisionMask = PhysicsLayers.GetOrMask(PhysicsLayers.WORLD, PhysicsLayers.RITUAL_INTERACT);
    }
    public void PhysicsProcess(float delta)
    {
        if (_currentRitual == null) return;

        Vector3 rayStart = _camera.GlobalPosition;
        Vector3 rayEnd = rayStart + -_camera.GlobalBasis.Z * MAX_DISTANCE;
        if (RaycastUtils.Ray(_player, rayStart, rayEnd, out Dictionary result, _collisionMask))
        {
            Vector3 position;
            ValidPosition = IsValidPosition();
            if (
                (Node)result["collider"] is Area3D a &&
                a.GetParent() is IRitualInteract interact &&
                interact.TryGetRitualPosition(_currentRitual, out position) &&
                ValidPosition
            )
            {
                ritualInteract = interact;
                position += Vector3.Up * 0.05f;
            }
            else
            {
                if (_currentRitual.RitualResource.RequiresInteract)
                    ValidPosition = false;
                Vector3 normal = result["normal"].AsVector3();
                position = result["position"].AsVector3() + normal * 0.05f;
                ritualInteract = null;
            }

            if (_initialSnap)
            {
                _currentRitual.GlobalPosition = position;
                _initialSnap = false;
            }
            else
                _currentRitual.GlobalPosition = _currentRitual.GlobalPosition.MoveToward(position, delta * 30f);

            Color currentColor = ValidPosition ? _previewColor : _errorColor;
            if (currentColor != _prevColor)
            {
                _prevColor = currentColor;
                _currentRitual.Renderer.ColorOverride = currentColor;
            }
        }
    }
    public void StartRitualPreview()
    {
        _initialSnap = true;
        RitualResource res = ResourceLoader.Load<RitualResource>("uid://c1e6c1npwbqxi");
        _currentRitual = RitualManagerNode.Instance.InstanciateRitualPreview(res, _player.GlobalPosition);
        _maxRadiusToCheckCollision = (_currentRitual.RitualResource.RitualCircles.Max(x => x.Radius) / 100) + COLLISION_CHECK_MARGIN;
        _currentRitual.Renderer.ColorOverride = _previewColor;
    }

    public bool IsValidPosition()
    {
        if (_currentRitual.GlobalPosition.DistanceTo(_player.GlobalPosition) > VALID_DISTANCE)
            return false;

        const int NUM_CHECKS = 16;
        Vector3 startPos = _currentRitual.GlobalPosition;
        return !RaycastUtils.Circle(_currentRitual, startPos, _maxRadiusToCheckCollision, NUM_CHECKS, out _, PhysicsLayers.WORLD);
    }

    public void Cancel()
    {
        RitualManagerNode.Instance.DisposeRitual(_currentRitual);
        _currentRitual = null;
    }

    private void AddTriggers()
    {
        _currentRitual.Triggers.Add(new SlotsFilledTrigger(_currentRitual.Slots));
    }

    public void BuildAndPlace()
    {
        RitualManagerNode.Instance.BuildElements(_currentRitual);
        AddTriggers();
        _currentRitual.SetIdle();
        _currentRitual.Renderer.ColorOverride = _placedColor;

        if (ritualInteract != null)
        {
            _currentRitual.OnComplete += ritualInteract.OnRitualComplete;
        }

        _currentRitual = null;
    }
}