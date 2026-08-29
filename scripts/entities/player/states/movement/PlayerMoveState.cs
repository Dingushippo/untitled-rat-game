using Godot;

public abstract partial class PlayerMoveState : PlayerState
{
    [Export] public float Speed;
    [Export] public float Acceleration;
    [Export] public float Deceleration;

    private float _speed;
    public override void PhysicsProcess(float delta)
    {
        if (Parent is not PlayerGroundedState grounded)
        {
            GD.PushError($"{this} is not a child of PlayerGroundedState"); return;
        }
        Parent.PhysicsProcess(delta);

        float desiredSpeed = Speed;
        float accel = Acceleration;

        if (grounded.Direction == Vector3.Zero)
        {
            desiredSpeed = 0f;
            accel = Deceleration;
        }

        _speed = Mathf.MoveToward(_speed, desiredSpeed, accel * delta);
        Vector3 horzontalVelocity = grounded.Direction * _speed;
        Vector3 newVelocity = new(
            horzontalVelocity.X,
            _velocity.Y,
            horzontalVelocity.Z
        );
        SetVelocity(newVelocity);
        MoveAndSlide();
    }
}
