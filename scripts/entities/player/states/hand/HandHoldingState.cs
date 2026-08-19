using Godot;

public class HandHoldingState : HandState
{
    public HandHoldingState(Hand owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        _hand.Held.GlobalPosition = _hand.GlobalPosition;
        _hand.Held.GlobalRotation = _hand.GlobalRotation;
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("throw"))
        {
            Vector3 direction = -_hand.Player.Camera.GlobalBasis.Z;
            _hand.Held.LookAt(_hand.ToGlobal(direction));
            _hand.Held.Freeze = false;
            _hand.Held.Collider.Disabled = false;
            _hand.Held.ApplyCentralImpulse(direction * 30f);
            _hand.Release();
            fsm.ChangeState<HandEmptyState>();
        }
    }
}
