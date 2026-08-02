using Godot;
using System.Collections.Generic;

public class RatLandedState : RatState
{
    private Tween _landTween;
    public RatLandedState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        Vector3 landingDirection = new Vector3(0, _rat.GlobalRotation.Y, 0);

        // TODO change to use raycast
        Vector3 landingPosition = _rat.GlobalPosition;
        landingPosition.Y = 0;

        _landTween = _rat.CreateTween();
        _landTween.SetParallel(true);
        _landTween.TweenProperty(_rat, "rotation", landingDirection, 0.35f);
        _landTween.Chain();
        _landTween.TweenCallback(Callable.From(() => SetNextState(previous)));
    }
    public override void Exit()
    {
        if (_landTween.IsRunning())
        {
            _landTween.Kill();
        }
    }

    private void SetNextState(State previous)
    {
        _rat.ResetTargetPosition();
        fsm.ChangeState("follow", this);
    }
}