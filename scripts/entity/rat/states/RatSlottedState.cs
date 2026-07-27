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
        slotTween.TweenProperty(_rat, "quaternion", LocalSlotRotation(), 0.35f);
        slotTween.TweenProperty(_rat, "global_position", _workSlot.GlobalPosition, 0.35f);

        _rat.SetNavAgentEnabled(false);

        // TODO Play animation associated with facility/slot

        EventBus.Publish(Event.RatSlotted, _workSlot.Facility, _workSlot, _rat);
    }
    public override void Exit()
    {
        _rat.SetNavAgentEnabled(true);

        if (_workSlot is not null)
        {
            _workSlot.Release();
        }
        EventBus.Publish(Event.RatUnslotted, _workSlot.Facility, _workSlot, _rat);
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