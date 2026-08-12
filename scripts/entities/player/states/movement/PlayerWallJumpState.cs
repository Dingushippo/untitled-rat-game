using Godot;

public class PlayerWallJumpState : PlayerState
{
    const float SLIDE_BOOST = 1.5f;
    public PlayerWallJumpState(Player owner) : base(owner) { }
    public override void Enter(State previous = null)
    {
        if (previous is not PlayerWallRunState wallRun)
            return;

        _player.Velocity = (wallRun.WallNormal + Vector3.Up) * _player.WallJumpForce;
        GD.Print($"Velocity: {_player.Velocity}");
        fsm.ChangeState("falling", this);
    }
}