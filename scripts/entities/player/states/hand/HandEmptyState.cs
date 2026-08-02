using Godot;
using System.Collections.Generic;

public class HandEmptyState : PlayerState
{
    public HandEmptyState(Player owner) : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        if (_player.GrabComponent.HasGrabbed())
        {
            fsm.ChangeState("grab");
        }
    }
}