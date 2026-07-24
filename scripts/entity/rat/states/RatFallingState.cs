public class RatFallingState : RatState
{
    public RatFallingState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        _rat.Velocity += _rat.GetGravity() * delta;
        _rat.MoveAndSlide();

        if (_rat.IsOnFloor())
        {
            fsm.ChangeState("landed", this);
        }
    }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}