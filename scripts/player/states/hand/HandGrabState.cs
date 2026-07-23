using Godot;

public class HandGrabState : PlayerState
{
    public HandGrabState(Player owner) : base(owner) { }

    public override void Enter(State previous = null)
    {
        _player.ThrowComponent.Enable();
    }
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("grab"))
        {
            if (_player.GrabComponent.HasGrabbed())
            {
                _player.GrabComponent.Release();
                fsm.ChangeState("empty");
            }

        }
        if (@event.IsActionPressed("throw"))
        {
            _player.ThrowComponent.StartDelayedCharge();
        }
        if (@event.IsActionReleased("throw"))
        {
            Rat ratToThrow = _player.GrabComponent.Retrieve();
            _player.ThrowComponent.Throw(ratToThrow);
            fsm.ChangeState("empty");
        }
    }
    public override void Exit()
    {
        _player.ThrowComponent.Reset();
    }
}