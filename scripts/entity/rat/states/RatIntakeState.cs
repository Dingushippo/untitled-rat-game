using Godot;

/// <summary>
/// Terminal state for an intake throw: the rat dumps whatever the facility's recipe wants
/// into its ingredient buffer, then drops back to normal behaviour.
/// </summary>
public class RatIntakeState : RatState
{
    private FacilityBase _facility;

    public RatIntakeState(Rat owner) : base(owner) { }

    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }

    public override void Enter(State previous = null)
    {
        if (previous is RatCurveState curve && curve.Target.IsIntake)
        {
            _facility = curve.Target.Facility;
        }

        if (_facility is null)
        {
            GD.PushWarning($"{_rat.Name} entered intake without a facility");
            fsm.ChangeState("falling", this);
            return;
        }

        var delivered = _facility.DeliverCargo(_rat);
        if (delivered.Count == 0)
        {
            GD.Print($"{_facility.Name} took nothing from {_rat.Name}");
        }

        fsm.ChangeState(_rat.IsOnFloor() ? "landed" : "falling", this);
    }

    public override void Exit()
    {
        _facility = null;
    }
}
