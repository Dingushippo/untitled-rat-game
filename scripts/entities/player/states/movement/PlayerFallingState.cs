using Godot;

public class PlayerFallingState : PlayerState
{
    const float COYOTY_TIMER_LENGTH = 0.2f;

    public PlayerFallingState(Player owner)
        : base(owner) { }

    private float _timer = 99f;
    private float _wallrunTimer = -2f;

    public override void PhysicsProcess(float delta)
    {
        _timer += delta;
        if (_wallrunTimer >= 0)
        {
            _wallrunTimer += delta;
        }

        _player.Direction = _player.GetCorrectedInput(sideToSideScaling: .3f);

        if (_player.IsOnFloor)
        {
            if (Input.IsActionPressed("crouch"))
                fsm.ChangeState<PlayerSlideState>(this);
            else
                fsm.ChangeState<PlayerMoveState>(this);
        }
        else if (CanWallrun())
        {
            fsm.ChangeState<PlayerWallRunState>(this);
        }
        else if (_player.VaultRaycast.IsColliding() && Input.IsActionPressed("jump") && CanVault())
        {
            fsm.ChangeState<PlayerVaultState>(this);
        }
        else if (Input.IsActionJustPressed("jump") && _timer <= COYOTY_TIMER_LENGTH)
        {
            fsm.ChangeState<PlayerJumpState>(this);
        }
    }

    public override void Enter(State previous = null)
    {
        if (previous is not PlayerJumpState)
            _timer = 0;
        _player.CrouchComponent.Enabled = false;
        // _player.HorizontalSpeed = _player.Tuning.Speed;
        // _player.HorizontalAccel = _player.Tuning.AirAcceleration;
        _wallrunTimer = previous is PlayerWallJumpState ? 0 : -1f;
    }

    public override void Exit()
    {
        _player.CrouchComponent.Enabled = true;
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
                _player.CollisionMask
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

        Vector3 position = _player.Camera.GlobalPosition;
        Vector3 rightOfPlayer =
            (position - _player.GlobalBasis.Z.Rotated(Vector3.Up, -Mathf.Pi / 2))
            * _player.Tuning.WallrunCheckDistance;
        Vector3 leftOfPlayer =
            (position - _player.GlobalBasis.Z.Rotated(Vector3.Up, Mathf.Pi / 2))
            * _player.Tuning.WallrunCheckDistance;

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
