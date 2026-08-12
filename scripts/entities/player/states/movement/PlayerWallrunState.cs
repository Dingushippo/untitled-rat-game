using Godot;
using Godot.Collections;

public class PlayerWallRunState : PlayerState
{
    private Side _side;
    public Vector3 WallNormal;
    public Vector3 WallForward;
    private float _gravityScale = -0.2f;
    private float _decayTime = 1.5f;
    private Tween _decayTween;
    public PlayerWallRunState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        if (_player.IsOnFloor())
        {
            fsm.ChangeState("idle"); return;
        }
        else if (!IsStillOnWall())
        {
            fsm.ChangeState("falling", this); return;
        }
        if (Input.IsActionJustPressed("jump"))
        {
            fsm.ChangeState("walljump", this); return;
        }
        Vector3 newVelocity = new(
            WallForward.X * _player.WallrunSpeed,
            _player.GetGravity().Y * _gravityScale * delta,
            WallForward.Z * _player.WallrunSpeed
        );
        newVelocity += -WallNormal * 3;
        _player.Velocity = newVelocity;
        _player.MoveAndSlide();

    }
    public override void Enter(State previous = null)
    {
        if (previous is PlayerFallingState fall)
        {
            _side = fall.Side;
            GD.Print($"Side: {_side}");
        }
        _decayTween?.Kill();
        _decayTween = _player.CreateTween();
        _decayTween.SetEase(Tween.EaseType.In);
        _decayTween.SetTrans(Tween.TransitionType.Circ);
        _decayTween.TweenMethod(Callable.From<float>((x) => _gravityScale = x), 1, 20f, _decayTime);
    }

    private bool IsStillOnWall()
    {
        Vector3 position = _player.Camera.GlobalPosition;
        Vector3 leftOfPlayer = (position - _player.GlobalBasis.Z.Rotated(Vector3.Up, Mathf.Pi / 2)) * _player.WallrunCheckDistance;
        Vector3 rightOfPlayer = (position - _player.GlobalBasis.Z.Rotated(Vector3.Up, -Mathf.Pi / 2)) * _player.WallrunCheckDistance;

        Vector3 posToCheck = _side == Side.Left ? leftOfPlayer : rightOfPlayer;
        int sign = _side == Side.Left ? -1 : 1;


        if (RaycastUtils.Ray(_player, position, posToCheck, out Dictionary result, PhysicsLayers.WORLD))
        {
            WallNormal = result["normal"].AsVector3();
            WallForward = sign * WallNormal.Cross(Vector3.Up);
            return true;
        }
        return false;
    }

}