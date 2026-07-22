using Godot;
using System.Linq;

public class PlayerFallingState : PlayerState
{
    const float COYOTY_TIMER_LENGTH = 0.2f;
    public PlayerFallingState(Player owner) : base(owner) { }


    private float _timer;
    public override void PhysicsProcess(float delta)
    {
        _timer += delta;
        HandleAirMovement(delta);

        _player.Velocity += _player.gravity * delta;

        _player.MoveAndSlide();

        if (_player.IsOnFloor())
        {
            fsm.ChangeState("move", this);
        }
        else if (Input.IsActionPressed("jump") && _timer <= COYOTY_TIMER_LENGTH)
        {
            fsm.ChangeState("jump", this);
        }
        else if (_player.VaultRaycast.IsColliding() && Input.IsActionPressed("jump") && CanVault())
        {
            fsm.ChangeState("vault", this);
        }

    }
    public override void Enter(State previous = null)
    {
        if (previous is not PlayerJumpState) _timer = 0;
    }

    private void HandleAirMovement(float delta)
    {
        Vector2 input = Input.GetVector(
            "move_left",
            "move_right",
            "move_forward",
            "move_back");

        if (input == Vector2.Zero)
            return;

        float speed = Input.IsActionPressed("sprint")
            ? _player.SprintSpeed
            : _player.Speed;

        float yaw = _player.Rotation.Y;

        Vector3 forward = new(Mathf.Sin(yaw), 0, Mathf.Cos(yaw));
        Vector3 right = new(forward.Z, 0, -forward.X);

        Vector3 desired =
            (right * input.X + forward * input.Y).Normalized();

        Vector3 velocity = _player.Velocity;
        Vector3 horizontal = new(velocity.X, 0, velocity.Z);

        // Smaller acceleration in the air
        horizontal = horizontal.MoveToward(
            desired * speed,
            _player.AirAcceleration * delta);

        velocity.X = horizontal.X;
        velocity.Z = horizontal.Z;

        _player.Velocity = velocity;
    }

    private bool CanVault()
    {
        Vector3 collisionPoint = _player.VaultRaycast.GetCollisionPoint();
        PhysicsDirectSpaceState3D state = _player.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            collisionPoint,
            collisionPoint + new Vector3(0, 2f, 0),
            _player.CollisionMask
        );
        return !state.IntersectRay(query).TryGetValue("collider", out _);
    }
}