using Godot;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player owner)
        : base(owner)
    {
        Reset();
    }

    private Vector3 _launchDirection;
    private float _launchForce;

    private void Reset()
    {
        _launchDirection = Vector3.Up;
        _launchForce = _player.Tuning.JumpForce;
    }

    public void Configure(Vector3 direction, float force)
    {
        _launchDirection = direction;
        _launchForce = force;
    }

    public override void Enter(State previous = null)
    {
        if (previous is not PlayerSwingState)
        {
            GD.Print("Not swing");
            Reset();
        }
        _player.StickToFloor = false;
        Vector3 jumpImpulse = _launchDirection * _launchForce;
        GD.Print($"Adding jump impulse: {jumpImpulse}");
        _player.SetImpulse(jumpImpulse);
        fsm.ChangeState<PlayerFallingState>(this);
    }
}
