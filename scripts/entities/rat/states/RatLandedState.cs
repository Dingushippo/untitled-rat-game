using Godot;
using Godot.Collections;
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

        if (RaycastUtils.Ray(
            _rat,
            _rat.GlobalPosition,
            _rat.GlobalPosition + Vector3.Down * 0.25f,
            out Dictionary result,
            PhysicsLayers.GetOrMask(PhysicsLayers.WORLD, PhysicsLayers.FACILITY),
            collideWithAreas: false
        ))
        {
            _rat.GlobalPosition = result["position"].AsVector3();
        }
        else
        {
            fsm.ChangeState<RatFallingState>();
        }

        _landTween = _rat.CreateTween();
        _landTween.SetParallel(true);
        _landTween.TweenProperty(_rat, "rotation", landingDirection, 0.35f);
        _landTween.Chain();
        _landTween.TweenCallback(Callable.From(() => SetNextState(previous)));
    }
    public override void Exit()
    {
        _landTween?.Kill();
    }

    private void SetNextState(State previous)
    {
        _rat.ResetTargetPosition();
        fsm.ChangeState<RatFollowState>(this);
    }
}