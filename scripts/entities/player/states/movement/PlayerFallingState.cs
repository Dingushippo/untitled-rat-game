using Godot;

public partial class PlayerFallingState : PlayerState
{
    const float COYOTY_TIMER_LENGTH = 0.2f;
    const float JUMP_BUFFER_LENGTH = 0.2f;

    private float _coyoteTimer = 99f;
    private float _jumpBufferTimer;
    private float _wallrunTimer = -2f;
    private bool _wantsJump;

    public override void PhysicsProcess(float delta)
    {
        Parent?.PhysicsProcess(delta);
        if (_player.IsOnFloor())
            _hfsm.ChangeState<PlayerGroundedState>();
        MoveAndSlide();
    }
    public override void Enter(State previous = null)
    {
    }

    private bool CanVault()
    {
        Vector3 collisionPoint = _player.VaultRaycast.GetCollisionPoint();
        if (
            RaycastUtils.Ray(
                _player,
                collisionPoint,
                collisionPoint + Vector3.Up * 2f,
                out _,
                PhysicsLayers.WORLD
            )
        )
        {
            return false;
        }
        return true;
    }

    public Side Side;

    private bool CanWallrun()
    {
        if (_player.Velocity.Y <= 0)
        {
            return false;
        }
        if (_wallrunTimer != -1 && _wallrunTimer < 0.4)
            return false;

        Vector3 rightDir = _player.Camera.GlobalBasis.X * _player.Tuning.WallrunCheckDistance;
        Vector3 position = _player.Camera.GlobalPosition + Vector3.Down; // About body center
        Vector3 rightOfPlayer = position + rightDir;
        Vector3 leftOfPlayer = position - rightDir;

        // Check left side
        if (RaycastUtils.Ray(_player, position, rightOfPlayer, out _, PhysicsLayers.WORLD))
        {
            Side = Side.Right;
            return true;
        }
        // Check right side
        else if (RaycastUtils.Ray(_player, position, leftOfPlayer, out _, PhysicsLayers.WORLD))
        {
            Side = Side.Left;
            return true;
        }
        return false;
    }
}
