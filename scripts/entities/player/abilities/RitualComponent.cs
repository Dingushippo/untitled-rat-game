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


    public RitualComponent(Player player)
    {
        _player = player;
    }
    public void PhysicsProcess(float delta)
    {
        if (_currentRitual == null) return;

        Vector3 rayStart = _camera.GlobalPosition;
        Vector3 rayEnd = rayStart + -_camera.GlobalBasis.Z * MAX_DISTANCE;
        if (Utils.Raycast(_player, rayStart, rayEnd, out Dictionary result, PhysicsLayers.WORLD, false))
        {
            Vector3 normal = result["normal"].AsVector3();
            Vector3 position = result["position"].AsVector3() + normal * 0.05f;
            _currentRitual.GlobalPosition = _currentRitual.GlobalPosition.MoveToward(position, delta * 80f);
            // _currentRitual.LookAt(_player.GlobalPosition);

            ValidPosition = IsValidPosition();
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
        for (int i = 0; i < NUM_CHECKS; i++)
        {
            float angle = i * (Mathf.Tau / NUM_CHECKS);
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 endPos = startPos + direction * _maxRadiusToCheckCollision;
            if (Utils.Raycast(_currentRitual, startPos, endPos, out _, PhysicsLayers.WORLD))
                return false;
        }
        return true;
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
        _currentRitual = null;
    }
}