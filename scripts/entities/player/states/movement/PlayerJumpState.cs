using Godot;

public class PlayerJumpState : PlayerState
{
    const float SLIDE_BOOST = 1.5f;

    public PlayerJumpState(Player owner)
        : base(owner) { }

    public override void Enter(State previous = null)
    {
        // Vector3 velocity = _player.Velocity;
        Vector3 jumpForce = Vector3.Up * _player.Tuning.JumpForce;
        // velocity.Y = _player.Tuning.JumpForce;
        if (previous is PlayerSlideState)
        {
            jumpForce.Y *= SLIDE_BOOST;
        }
        // _player.Velocity = velocity;
        // _player.CrouchComponent.TryStand();
        _player.ApplyCentralImpulse(jumpForce);
        fsm.ChangeState<PlayerFallingState>(this);
    }
}
