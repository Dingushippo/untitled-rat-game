public partial class PlayerIdleState : PlayerState
{

    public override void PhysicsProcess(float delta)
    {

        _parent.PhysicsProcess(delta);
    }

}
