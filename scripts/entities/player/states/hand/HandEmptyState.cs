using Godot;

public class HandEmptyState : HandState
{
    public HandEmptyState(Player owner) : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (_player.GrabComponent.HasGrabbed())
        {
            fsm.ChangeState<HandGrabState>(this);
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ritual"))
        {
            fsm.ChangeState<HandRitualState>();
        }
    }
}