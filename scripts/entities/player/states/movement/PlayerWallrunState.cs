using Godot;
using Godot.Collections;

public class PlayerWallRunState : PlayerState
{
    private Side _side;
    public Vector3 WallNormal;
    public Vector3 WallForward;
    private float _gravityScale = 0.5f;

    public PlayerWallRunState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (_player.IsOnFloor)
        {
            fsm.ChangeState<PlayerIdleState>();
            return;
        }
        else if (!IsStillOnWall())
        {
            fsm.ChangeState<PlayerFallingState>(this);
            return;
        }
        if (Input.IsActionJustPressed("jump"))
        {
            fsm.ChangeState<PlayerWallJumpState>(this);
            return;
        }
        Vector3 velocity = _player.Velocity;
        velocity.X = WallForward.X * _player.Tuning.WallrunSpeed;
        velocity.Z = WallForward.Z * _player.Tuning.WallrunSpeed;
        velocity.Y += _player.GetGravity().Y * _gravityScale * delta;

        // Stick to wall to make it more stable
        velocity += -WallNormal * 3; // Maybe make this a tuning variable?

        _player.Velocity = velocity;
    }

    public override void Enter(State previous = null)
    {
        if (previous is PlayerFallingState fall)
        {
            _side = fall.Side;
        }
        _player.Camera.SetLean(_side);
    }

    public override void Exit()
    {
        _player.Camera.ResetPose();
    }

    private bool IsStillOnWall()
    {
        Vector3 position = _player.Camera.GlobalPosition;
        Vector3 leftOfPlayer =
            (position - _player.GlobalBasis.Z.Rotated(Vector3.Up, Mathf.Pi / 2))
            * _player.Tuning.WallrunCheckDistance;
        Vector3 rightOfPlayer =
            (position - _player.GlobalBasis.Z.Rotated(Vector3.Up, -Mathf.Pi / 2))
            * _player.Tuning.WallrunCheckDistance;

        Vector3 posToCheck = _side == Side.Left ? leftOfPlayer : rightOfPlayer;
        int sign = _side == Side.Left ? -1 : 1;

        if (
            RaycastUtils.Ray(
                _player,
                position,
                posToCheck,
                out Dictionary result,
                PhysicsLayers.WORLD
            )
        )
        {
            WallNormal = result["normal"].AsVector3();
            WallForward = sign * WallNormal.Cross(Vector3.Up);
            return true;
        }
        return false;
    }
}
