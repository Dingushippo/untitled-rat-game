using Godot;

public class HandEmptyState : HandState
{
    public HandEmptyState(Hand owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta) { }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("throw"))
        {
            if (!RatManager.Instance.TrySpawnRat(out Rat rat, _hand.GlobalPosition))
                return;

            Vector3 direction = -_hand.Player.Camera.GlobalBasis.Z;
            rat.LookAt(_hand.ToGlobal(direction));
            rat.Freeze = false;
            rat.Collider.Disabled = false;
            rat.ApplyCentralImpulse(direction * 30f);
            fsm.ChangeState<HandEmptyState>();
        }
    }
}
