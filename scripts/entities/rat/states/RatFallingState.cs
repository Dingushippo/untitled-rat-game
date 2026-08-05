public class RatFallingState : RatState
{
    public RatFallingState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta)
    {
        if (_rat.Collider.Disabled && Utils.ShapeCast(_rat, _rat.Collider, out _, PhysicsLayers.GetOrMask(PhysicsLayers.WORLD, PhysicsLayers.FACILITY), false))
        {
            _rat.Collider.Disabled = false;
        }

        _rat.Velocity += _rat.GetGravity() * _rat.FlightTuning.DescentGravityScale * delta;
        _rat.MoveAndSlide();

        if (_rat.IsOnFloor())
        {
            fsm.ChangeState("landed", this);
        }
    }
}