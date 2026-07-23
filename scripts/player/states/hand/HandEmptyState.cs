using Godot;
using System.Collections.Generic;

public class HandEmptyState : PlayerState
{
    public HandEmptyState(Player owner) : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        _player.GrabComponent.PhysicsUpdate();
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("grab"))
        {
            if (_player.GrabComponent.TryGrab())
            {
                fsm.ChangeState("grab");
            }
        }
    }
}