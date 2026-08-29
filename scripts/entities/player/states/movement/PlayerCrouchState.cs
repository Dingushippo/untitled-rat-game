using Godot;

public partial class PlayerCrouchState : PlayerMoveState
{
    [Export] private float _crouchOffset = -0.6f;
    [Export] private float _crouchAnimationDuration = 0.15f;
    private Tween _crouchTween;
    private float _colliderHeight;
    private float _colliderYPos;

    public override void Init(Node owner, HierarchicalStateMachine<PlayerState> stateMachine, State parent)
    {
        base.Init(owner, stateMachine, parent);
        _colliderHeight = (_player.Collider.Shape as CapsuleShape3D).Height;
        _colliderYPos = _player.Collider.Position.Y;
    }

    public override void PhysicsProcess(float delta)
    {
        base.PhysicsProcess(delta);
        if (!_player.Input.WantsCrouch)
            _hfsm.ChangeState<PlayerIdleState>();
    }
    public override void Enter(State previous = null)
    {
        TweenCrouchOffset(_crouchOffset);
    }

    public override void Exit()
    {
        TweenCrouchOffset(0f);
    }

    private void TweenCrouchOffset(float height)
    {
        if (_crouchTween is not null)
        {
            _crouchTween.Kill();
        }
        _crouchTween = _player.CreateTween();
        _crouchTween.SetParallel(true);
        _crouchTween.TweenProperty(_player.Camera, "YOffset", height, _crouchAnimationDuration);
        _crouchTween.TweenProperty(_player.Collider.Shape as CapsuleShape3D, "height", _colliderHeight + height, _crouchAnimationDuration);
        _crouchTween.TweenProperty(_player.Collider, "position:y", _colliderYPos + height / 2, _crouchAnimationDuration);
    }
}