public partial class PlayerIdleState : PlayerState
{
    public override void Enter(State previous = null)
    {
        _player.Camera.SetBobVariables(0f, 0f);
    }
}
