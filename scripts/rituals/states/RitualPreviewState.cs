using Godot;


public class RitualPreviewState : RitualState
{
    public RitualPreviewState(RitualBase owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}