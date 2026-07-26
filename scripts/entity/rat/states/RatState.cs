public class RatState : State
{
    private protected Rat _rat;

    public RatState(Rat owner) { _rat = owner; }
}

/* Template

using Godot;


public class RatNewState : RatState
{
    public RatNewState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}

*/