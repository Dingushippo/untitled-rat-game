using Godot;

public class PlayerJumpState : PlayerState
{
    const float SLIDE_BOOST = 1.5f;
    public PlayerJumpState(Player owner) : base(owner) { }
    public override void Enter(State previous = null)
    {
        Vector3 velocity = _player.Velocity;
        velocity.Y = _player.JumpForce;
        if (previous is PlayerSlideState)
        {
            velocity.Y *= SLIDE_BOOST;
        }
        _player.Velocity = velocity;
        _player.CrouchComponent.TryStand();
        fsm.ChangeState("falling", this);
    }
}