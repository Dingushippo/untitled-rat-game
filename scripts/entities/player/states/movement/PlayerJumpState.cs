using Godot;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player owner)
        : base(owner) { }

    public override void Enter(State previous = null)
    {
        Vector3 jumpForce = Vector3.Up * _player.Tuning.JumpForce;

        if (previous is PlayerSlideState)
        {
            jumpForce.Y *= _player.Tuning.SlideJumpBoost;
        }

        _player.ApplyCentralImpulse(jumpForce);
        fsm.ChangeState<PlayerFallingState>(this);
    }
}
