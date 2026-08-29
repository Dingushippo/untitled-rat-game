using Godot;

public class HandEmptyState : HandState
{
    public HandEmptyState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta) { }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("right_hand"))
        {
            Vector3 direction = -_player.Camera.GlobalBasis.Z;
            if (
                !RatManager.Instance.TrySpawnRat(
                    out Rat rat,
                    _player.HandR.GlobalPosition + direction * 0.1f
                )
            )
                return;

            rat.LookAt(_player.GlobalPosition + direction * 5f);
            rat.Freeze = false;
            rat.Collider.Disabled = false;
            rat.ApplyCentralImpulse(direction * 30f);
            _hfsm.ChangeState<HandEmptyState>();
        }

        if (@event.IsActionPressed("left_hand") && !_player.Whip.IsAnchored)
        {
            _player.Whip.EngageWhip();
        }
        if (@event.IsActionReleased("left_hand") && _player.Whip.IsAnchored)
        {
            _player.Whip.Release();
        }
    }
}
