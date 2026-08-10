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
        // Play exit animation, and then despawn the parent

        RitualManagerNode.Instance.DisposeRitual(_ritual);
    }
    public override void Exit() { }
}