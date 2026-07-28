using Godot;

public class RatIdleState : RatState
{
    public RatIdleState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        if (!_rat.IsOnFloor())
        {
            fsm.ChangeState("falling");
        }
    }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {

    }
    public override void Exit() { }
}