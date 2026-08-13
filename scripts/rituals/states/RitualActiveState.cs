using Godot;


public class RitualActiveState : RitualState
{
    private float _timer;
    public RitualActiveState(RitualBase owner) : base(owner) { }
    public override void Process(float delta)
    {
        if (_timer < _ritual.RitualResource.RitualTime)
        {
            _timer += delta;
            return;
        }
        fsm.ChangeState<RitualCompletedState>();
    }
    public override void Enter(State previous = null)
    {
        _ritual.AnimateRats();
    }
    public override void Exit()
    {
        _ritual.StopAnimation();
    }
}