using Godot;

public class HandRitualState : PlayerState
{
    public HandRitualState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        _player.RitualComponent.PhysicsProcess(delta);
    }
    public override void Enter(State previous = null)
    {
        _player.RitualComponent.StartRitualPreview();
    }
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("throw") && _player.RitualComponent.ValidPosition)
        {
            _player.RitualComponent.BuildAndPlace();
            fsm.ChangeState<HandEmptyState>(this);
        }

        if (@event.IsActionPressed("ritual") && _player.RitualComponent.ValidPosition)
        {
            _player.RitualComponent.Cancel();
            fsm.ChangeState<HandEmptyState>(this);
        }
    }
}