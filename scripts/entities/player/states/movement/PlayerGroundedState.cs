
using Godot;

public partial class PlayerGroundedState : PlayerState
{
    public Vector3 Direction;
    public override void PhysicsProcess(float delta)
    {
        _parent?.PhysicsProcess(delta);

        if (!_player.IsOnFloor())
        {
            _hfsm.ChangeState<PlayerFallingState>();
            return;
        }
        if (!_player.Input.WantsJump)
        {
            _hfsm.ChangeState<PlayerJumpState>();
            return;
        }

        Direction = _player.Input.Direction;
    }
}
