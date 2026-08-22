using Godot;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player owner)
        : base(owner) { }

    public override void Enter(State previous = null)
    {
        if (previous is not PlayerWallRunState wallRun)
            return;

        _player.Velocity = (wallRun.WallNormal + Vector3.Up) * _player.Tuning.WallJumpForce;
        fsm.ChangeState<PlayerFallingState>(this);
    }
}
