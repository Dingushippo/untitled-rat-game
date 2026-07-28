using Godot;
using System.Collections.Generic;

public class HandEmptyState : PlayerState
{
    public HandEmptyState(Player owner) : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        _player.GrabComponent.PhysicsUpdate();
        if (_player.GrabComponent.HasGrabbed())
        {
            fsm.ChangeState("grab");
        }
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            // Facilities and other handlers claim the key first; picking up a rat is the fallback.
            if (_player.InteractComponent.IsLookingAtHandler) return;

            if (_player.GrabComponent.TryGrab())
            {
                fsm.ChangeState("grab");
            }
        }
    }
}