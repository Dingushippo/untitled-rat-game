using Godot;

public class HandEmptyState : HandState
{
    public HandEmptyState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta) { }
}
