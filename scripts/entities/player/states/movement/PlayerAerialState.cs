
using Godot;

public partial class PlayerAerialState : PlayerState
{
    [Export] public float AirSpeed;
    [Export] public float AirAcceleration;

    internal Vector3 Direction;
    public override void PhysicsProcess(float delta)
    {
        Direction = _player.Input.Direction;

        Vector3 targetVelocity = Direction * AirSpeed;
        Vector3 currentHorizontal = new Vector3(_velocity.X, 0, _velocity.Z);
        Vector3 newHorizontal = currentHorizontal.MoveToward(targetVelocity, AirAcceleration * delta);

        SetVelocity(new Vector3(newHorizontal.X, _player.Velocity.Y, newHorizontal.Z));
        AddVelocity(_player.GetGravity() * delta);
    }

    public override void Enter(State previous = null)
    {
        _player.Camera.SetBobVariables(0f, 0f);
    }
}
