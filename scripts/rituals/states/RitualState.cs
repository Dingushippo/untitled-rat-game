public class RitualState : State
{
    private protected RitualBase _ritual;

    public RitualState(RitualBase owner) { _ritual = owner; }
}

/* Template

using Godot;


public class RitualNewState : RitualState
{
    public RitualNewState(RitualBase owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}

*/