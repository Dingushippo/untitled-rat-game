using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GrapplePullAbility : MovementAbility
{

    [Export] public float PullSpeed = 25f;
    [Export] public float Range = 30f;
    private Vector3 _targetPoint;
    public override void OnActivate()
    {
        Vector3 startPos = _player.GlobalPosition;
        Vector3 endPos = startPos - _player.Camera.GlobalBasis.Z * Range;
        if (RaycastUtils.Ray(_player, startPos, endPos, out Dictionary result, PhysicsLayers.WORLD))
        {
            _targetPoint = result["position"].AsVector3();
        }
        else
        {
            _hfsm.ChangeState<PlayerFallingState>();
        }
    }

    public override void OnDeactivate()
    {
        // throw new System.NotImplementedException();
    }

    public override void PhysicsProcess(float delta)
    {
        Vector3 direction = _player.GlobalPosition.DirectionTo(_targetPoint);

        _player.Velocity = direction * PullSpeed;
        _player.MoveAndSlide();

        if (_player.GlobalPosition.DistanceTo(_targetPoint) < 2.0f || !_player.Input.LeftArmAction)
        {
            _hfsm.ChangeState<PlayerJumpState>();
        }
    }
}