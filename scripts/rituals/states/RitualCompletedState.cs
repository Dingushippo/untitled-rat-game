using Godot;


public class RitualCompletedState : RitualState
{
    public RitualCompletedState(RitualBase owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        foreach (RitualElementSlot slot in _ritual.Slots)
        {
            slot.WorkSlot.Occupant.ForceState("follow");
        }
    }
    public override void Exit() { }
}