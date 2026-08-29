using Godot;

public partial class PlayerJumpState : PlayerState
{
    [Export] public float JumpForce;
    [Export] private int _skipFrames;

    private int _framesElapsed;

    public override void PhysicsProcess(float delta)
    {
        if (_player.Velocity.Y <= 0)
        {
            _hfsm.ChangeState<PlayerFallingState>();
        }
        MoveAndSlide();
        _framesElapsed++;
    }


    public override void Enter(State previous = null)
    {
        _framesElapsed = 0;
        AddVelocity(Vector3.Up * JumpForce);
    }
}
