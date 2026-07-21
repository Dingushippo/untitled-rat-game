using Godot;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player owner) : base(owner) { }

    public override void Enter(State previous = null)
    {
        Vector3 velocity = _player.Velocity;
        velocity.Y = _player.JumpForce;
        _player.Velocity = velocity;

        fsm.ChangeState("falling", this);
    }
}