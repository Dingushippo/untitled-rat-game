using Godot;

public partial class PlayerCrouchState : PlayerMoveState
{
    [Export] private float _crouchOffset = -0.6f;
    [Export] private float _crouchAnimationDuration = 0.15f;

    private Tween _crouchTween;
    private CapsuleShape3D _capsuleShape;
    private float _standingHeight;
    private float _standingYPos;

    public override void Init(Node owner, HierarchicalStateMachine stateMachine, State parent)
    {
        base.Init(owner, stateMachine, parent);

        // Make shape unique to prevent mutating shared resource
        if (_player.Collider.Shape is CapsuleShape3D shape)
        {
            _capsuleShape = (CapsuleShape3D)shape.Duplicate();
            _player.Collider.Shape = _capsuleShape;
            _standingHeight = _capsuleShape.Height;
        }

        _standingYPos = _player.Collider.Position.Y;
    }

    public override void PhysicsProcess(float delta)
    {
        if (Parent is not PlayerGroundedState grounded)
            return;

        base.PhysicsProcess(delta);

        if (!_player.InputComponent.WantsCrouch && CanStand())
        {
            if (grounded.Direction != Vector3.Zero)
                _hfsm.ChangeState<PlayerRunState>();
            else
                _hfsm.ChangeState<PlayerIdleState>();
        }
    }

    public override void Enter(State previous = null)
    {
        base.Enter(previous);
        TweenCrouchOffset(_standingYPos + (_crouchOffset / 2f), _crouchOffset);
    }

    public override void Exit()
    {
        base.Exit();
        TweenCrouchOffset(_standingHeight, 0);
    }

    private bool CanStand()
    {
        // Cast from top of current crouched collision to top of standing height
        float currentCrouchHeight = _capsuleShape.Height;
        float heightDifference = _standingHeight - currentCrouchHeight;

        Vector3 startPos = _player.GlobalPosition + (Vector3.Up * currentCrouchHeight);
        Vector3 endPos = startPos + (Vector3.Up * (heightDifference + 0.05f));

        return !RaycastUtils.Ray(_player, startPos, endPos, out _, PhysicsLayers.WORLD);
    }

    private void TweenCrouchOffset(float targetHeight, float yOffset)
    {
        _crouchTween?.Kill();
        _crouchTween = _player.CreateTween().SetParallel(true);

        _crouchTween.TweenProperty(_player.Camera, "YOffset", yOffset, _crouchAnimationDuration);
        _crouchTween.TweenProperty(_capsuleShape, "height", targetHeight, _crouchAnimationDuration);
        _crouchTween.TweenProperty(_player.Collider, "position:y", targetHeight / 2, _crouchAnimationDuration);
    }
}