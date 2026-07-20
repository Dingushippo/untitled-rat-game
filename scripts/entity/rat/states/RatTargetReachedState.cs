public class RatTargetReachedState : RatState
{
    public RatTargetReachedState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        _rat.RotateY(delta * 1f);
    }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}