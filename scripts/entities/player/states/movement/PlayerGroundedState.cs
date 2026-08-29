
using Godot;

public partial class PlayerGroundedState : PlayerState
{
    public Vector3 Direction;
    public override void PhysicsProcess(float delta)
    {
        _parent?.PhysicsProcess(delta);

        Direction = _player.Input.Direction;

        if (!_player.IsOnFloor())
            _hfsm.ChangeState<PlayerFallingState>();
        else if (_player.Input.WantsJump)
            _hfsm.ChangeState<PlayerJumpState>();
        else if (_player.Input.WantsCrouch && !_hfsm.IsState<PlayerCrouchState>())
            _hfsm.ChangeState<PlayerCrouchState>();
        else if (Direction != Vector3.Zero && !_hfsm.IsState<PlayerRunState>())
            _hfsm.ChangeState<PlayerRunState>();

    }
}
