using Godot;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player owner)
        : base(owner) { }

    public override void Enter(State previous = null)
    {
        Vector3 jumpImpulse = Vector3.Up * _player.Tuning.JumpForce;
        _player.SetImpulse(jumpImpulse);
        fsm.ChangeState<PlayerFallingState>(this);
    }
}
