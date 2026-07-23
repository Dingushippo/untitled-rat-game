using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerFallingState : PlayerState
{
    const float COYOTY_TIMER_LENGTH = 0.2f;
    public PlayerFallingState(Player owner) : base(owner) { }


    private float _timer = 99f;
    public override void PhysicsProcess(float delta)
    {
        _timer += delta;
        HandleAirMovement(delta);

        _player.Velocity += _player.Gravity * delta;

        _player.MoveAndSlide();

        if (_player.IsOnFloor())
        {
            fsm.ChangeState("move", this);
        }
        if (_player.VaultRaycast.IsColliding() && Input.IsActionPressed("jump") && CanVault())
        {
            fsm.ChangeState("vault", this);
        }
        else if (Input.IsActionJustPressed("jump") && _timer <= COYOTY_TIMER_LENGTH)
        {
            fsm.ChangeState("jump", this);
        }


    }
    public override void Enter(State previous = null)
    {
        if (previous is not PlayerJumpState) _timer = 0;
    }

    private void HandleAirMovement(float delta)
    {
        Vector3 velocity = _player.GetMovementInputVelocity(_player.AirAcceleration, delta);
        if (velocity == Vector3.Zero) return;
        _player.Velocity = velocity;
    }

    private bool CanVault()
    {
        Vector3 collisionPoint = _player.VaultRaycast.GetCollisionPoint();
        if (Utils.Raycast(_player, collisionPoint, collisionPoint + Vector3.Up * 2f, out _, _player.CollisionMask))
        {
            return false;
        }
        return true;
    }
}