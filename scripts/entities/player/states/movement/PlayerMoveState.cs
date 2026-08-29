using Godot;

public abstract partial class PlayerMoveState : PlayerState
{
    [Export] public float Speed;
    [Export] public float Acceleration;
    [Export] public float Deceleration;

    [ExportGroup("Headbob")]
    [Export] public float BobSpeed;
    [Export] public float BobStrength;

    private float _speed;
    private Vector3 _cachedDirection;
    public override void PhysicsProcess(float delta)
    {
        if (Parent is not PlayerGroundedState grounded)
        {
            GD.PushError($"{this} is not a child of PlayerGroundedState"); return;
        }

        float desiredSpeed = Speed;
        float accel = Acceleration;

        if (grounded.Direction == Vector3.Zero)
        {
            desiredSpeed = 0f;
            accel = Deceleration;
        }
        else
            _cachedDirection = grounded.Direction;

        _speed = Mathf.MoveToward(_speed, desiredSpeed, accel * delta);
        Vector3 horzontalVelocity = _cachedDirection * _speed;
        Vector3 newVelocity = new(
            horzontalVelocity.X,
            _velocity.Y,
            horzontalVelocity.Z
        );

        SetVelocity(newVelocity);
        MoveAndSlide();
    }

    public override void Enter(State previous = null)
    {
        _speed = _velocity.Length();
        _player.Camera.SetBobVariables(BobSpeed, BobStrength);
    }
}
