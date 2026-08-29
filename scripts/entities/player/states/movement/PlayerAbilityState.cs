public partial class PlayerAbilityState : PlayerMoveState
{
    public MovementAbility ActiveAbility { get; set; }

    public override void Enter(State previous = null)
    {
        if (ActiveAbility == null)
        {
            _hfsm.ChangeState<PlayerFallingState>();
            return;
        }

        ActiveAbility.OnActivate();
    }

    public override void PhysicsProcess(float delta)
    {
        ActiveAbility.PhysicsProcess(delta);
    }

    public override void Exit()
    {
        ActiveAbility?.OnDeactivate();
    }
}