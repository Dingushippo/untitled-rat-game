using Godot;

public partial class PlayerMoveState : PlayerState
{
    [Export] public float Speed;
    [Export] public float Acceleration;
    [Export] public float Deceleration;

    private float _speed;
    public override void PhysicsProcess(float delta)
    {
        if (_parent is not PlayerGroundedState grounded)
        {
            GD.PushError($"{this} is not a child of PlayerGroundedState"); return;
        }

        grounded.PhysicsProcess(delta);

        float desiredSpeed = Speed;
        float accel = Acceleration;

        if (grounded.Direction == Vector3.Zero)
        {
            desiredSpeed = 0f;
            accel = Deceleration;
        }

        _speed = Mathf.MoveToward(_speed, desiredSpeed, accel * delta);
        SetVelocity(grounded.Direction * _speed);
        MoveAndSlide();

        if (_velocity.IsEqualApprox(Vector3.Zero))
        {
            _hfsm.ChangeState<PlayerIdleState>();
        }
    }

    public override void Exit() { }

    public override void Enter(State previous = null)
    {
    }
}
