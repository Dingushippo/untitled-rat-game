using Godot;
using System.Linq;


public class RitualIdleState : RitualState
{
    public RitualIdleState(RitualBase owner) : base(owner) { }
    public override void Enter(State previous = null)
    {
        foreach (IRitualTrigger trigger in _ritual.Triggers)
        {
            trigger.OnFulfilled += CheckValidTriggers;
        }
    }
    public override void Exit()
    {
        foreach (IRitualTrigger trigger in _ritual.Triggers)
            trigger.OnFulfilled -= CheckValidTriggers;
    }

    public void CheckValidTriggers()
    {
        if (_ritual.Triggers.All(x => x.IsFulfilled))
            fsm.ChangeState<RitualActiveState>();
    }
}