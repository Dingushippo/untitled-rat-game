using Godot;

public class HandGrabState : HandState
{
    public HandGrabState(Player owner) : base(owner) { }

    public override void Enter(State previous = null)
    {
        _player.ThrowComponent.Enable();
    }
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("interact"))
        {
            // "interact" doubles as grab/drop: if the crosshair is on something that handles
            // interaction, that wins and the rat stays in hand.
            if (_player.InteractComponent.IsLookingAtHandler) return;

            if (_player.GrabComponent.HasGrabbed())
            {
                _player.GrabComponent.Release();
                fsm.ChangeState<HandEmptyState>(this);
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
            fsm.ChangeState<HandEmptyState>(this);
        }
    }
    public override void Exit()
    {
        _player.ThrowComponent.Reset();
    }
}