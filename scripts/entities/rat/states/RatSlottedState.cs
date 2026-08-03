using Godot;
using System.Diagnostics.Tracing;


public class RatSlottedState : RatState
{
    private WorkSlot _workSlot;
    public RatSlottedState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        if (previous is RatCurveState state)
        {
            _workSlot = state.WorkSlot;
        }

        Tween slotTween = _rat.CreateTween();
        slotTween.SetParallel(true);
        slotTween.TweenProperty(_rat, "quaternion", LocalSlotRotation(), _rat.FlightTuning.SettleDuration);
        slotTween.TweenProperty(_rat, "global_position", _workSlot.GlobalPosition, _rat.FlightTuning.SettleDuration);

        // _rat.SetNavAgentEnabled(false);

        // TODO Play animation associated with facility/slot

        EventBus.Publish(Event.RatSlotted, _workSlot.Facility, _workSlot, _rat);
    }
    public override void Exit()
    {
        // _rat.SetNavAgentEnabled(true);

        if (_workSlot is null) return;

        FacilityBase facility = _workSlot.Facility;
        _workSlot.Release();
        EventBus.Publish(Event.RatUnslotted, facility, _workSlot, _rat);
    }

    /// <summary>Slot facing expressed in the rat's parent space, as a quaternion so the tween slerps
    /// the short way round instead of unwinding Euler angles.</summary>
    private Quaternion LocalSlotRotation()
    {
        Basis target = _workSlot.GlobalBasis.Orthonormalized();

        if (_rat.GetParent() is Node3D parent)
        {
            target = parent.GlobalBasis.Orthonormalized().Inverse() * target;
        }

        return target.GetRotationQuaternion();
    }
}