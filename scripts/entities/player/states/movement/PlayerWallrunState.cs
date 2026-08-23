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
    }

    public override void Enter(State previous = null)
    {
        if (previous is PlayerFallingState fall)
        {
            _side = fall.Side;
        }

        IsStillOnWall();
        _player.Direction = WallForward;
        _player.HorizontalSpeed = _player.Tuning.WallrunSpeed;
        _player.Camera.SetLean(_side);
    }

    public override void Exit()
    {
        _player.Camera.ResetPose();
    }

    private bool IsStillOnWall()
    {
        Vector3 rightDir = _player.Head.GlobalBasis.X * _player.Tuning.WallrunCheckDistance;
        Vector3 position = _player.Head.GlobalPosition + Vector3.Down; // About body center
        Vector3 rightOfPlayer = position + rightDir;
        Vector3 leftOfPlayer = position - rightDir;

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
