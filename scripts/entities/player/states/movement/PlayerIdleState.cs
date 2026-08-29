public partial class PlayerIdleState : PlayerState
{

    public override void PhysicsProcess(float delta)
    {
        Parent.PhysicsProcess(delta);
    }

}
