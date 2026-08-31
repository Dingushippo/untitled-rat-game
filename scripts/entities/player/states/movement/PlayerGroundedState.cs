using Godot;

public partial class PlayerGroundedState : PlayerState
{
    public Vector3 Direction;
    [Export] public float CoyotyTime = 0.15f;

    public override void PhysicsProcess(float delta)
    {
        Direction = _player.InputComponent.Direction;

        // 1. High-Priority Air Transitions
        if (!_player.IsOnFloor())
        {
            _hfsm.ChangeState<PlayerFallingState>();
            return;
        }

        if (_player.InputComponent.WantsJump)
        {
            _player.InputComponent.ConsumeJump();
            _hfsm.ChangeState<PlayerJumpState>();
            return;
        }

        // 2. Slide Transition (Triggered when sprinting + crouch/slide input)
        if (_player.InputComponent.WantsCrouch && CanSlide())
        {
            _hfsm.ChangeState<PlayerSlideState>();
            return;
        }

        // 3. Crouch Transition
        if (_player.InputComponent.WantsCrouch && !_hfsm.IsState<PlayerCrouchState>() && !_hfsm.IsState<PlayerSlideState>())
        {
            _hfsm.ChangeState<PlayerCrouchState>();
            return;
        }

        // 4. Ground Movement State Matrix (Only evaluated if not Crouching or Sliding)
        if (!_hfsm.IsState<PlayerCrouchState>() && !_hfsm.IsState<PlayerSlideState>())
        {
            EvaluateGroundMovementState();
        }
    }

    private void EvaluateGroundMovementState()
    {
        bool hasInput = Direction != Vector3.Zero;

        if (_player.Velocity.IsZeroApprox() && !hasInput)
        {
            if (!_hfsm.IsState<PlayerIdleState>())
                _hfsm.ChangeState<PlayerIdleState>();
        }
        else if (_player.InputComponent.WantsSprint)
        {
            if (!_hfsm.IsState<PlayerSprintState>())
                _hfsm.ChangeState<PlayerSprintState>();
        }
        else
        {
            if (!_hfsm.IsState<PlayerRunState>())
                _hfsm.ChangeState<PlayerRunState>();
        }
    }

    private bool CanSlide()
    {
        // Require active movement and sprint state/speed threshold to initiate a slide
        bool isSprinting = _hfsm.IsState<PlayerSprintState>();
        return isSprinting && Direction != Vector3.Zero;
    }
}