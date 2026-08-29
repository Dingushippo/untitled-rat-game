using Godot;

public class PlayerFallingState : PlayerState
{
    const float COYOTY_TIMER_LENGTH = 0.2f;
    const float JUMP_BUFFER_LENGTH = 0.2f;

    public PlayerFallingState(Player owner)
        : base(owner) { }

    private float _coyoteTimer = 99f;
    private float _jumpBufferTimer;
    private float _wallrunTimer = -2f;
    private bool _wantsJump;

    public override void PhysicsProcess(float delta)
    {
        _coyoteTimer += delta;
        if (_wallrunTimer >= 0)
            _wallrunTimer += delta;
        if (_wantsJump)
            _jumpBufferTimer += delta;

        _player.Direction = _player.GetCorrectedInput(sideToSideScaling: .3f);
        if (_player.IsOnFloor)
        {
            if (_wantsJump && _jumpBufferTimer <= JUMP_BUFFER_LENGTH)
            {
                _hfsm.ChangeState<PlayerJumpState>(this);
            }
            else if (Input.IsActionPressed("crouch"))
                _hfsm.ChangeState<PlayerSlideState>(this);
            else
                _hfsm.ChangeState<PlayerMoveState>(this);
        }
        else if (CanWallrun())
        {
            _hfsm.ChangeState<PlayerWallRunState>(this);
        }
        else if (_player.VaultRaycast.IsColliding() && Input.IsActionPressed("jump") && CanVault())
        {
            _hfsm.ChangeState<PlayerVaultState>(this);
        }
        else if (Input.IsActionJustPressed("jump") && (_coyoteTimer <= COYOTY_TIMER_LENGTH))
        {
            _hfsm.ChangeState<PlayerJumpState>(this);
        }
    }

    public override void Enter(State previous = null)
    {
        if (previous is not PlayerJumpState && previous is not PlayerSwingState)
            _coyoteTimer = 0;
        _player.CrouchComponent.Enabled = false;
        _wallrunTimer = previous is PlayerWallJumpState ? 0 : -1f;
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
            _wantsJump = true;
    }

    public override void Exit()
    {
        _player.CrouchComponent.Enabled = true;
        _wantsJump = false;
        _jumpBufferTimer = 0;
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
        if (_player.LinearVelocity.Y <= 0)
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
