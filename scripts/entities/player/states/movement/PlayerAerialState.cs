using Godot;

public partial class PlayerAerialState : PlayerState
{
    [Export] public float AirSpeed = 7.0f;          // Target speed applied by WASD inputs
    [Export] public float AirAcceleration = 35.0f;  // Responsiveness of air control
    [Export] public float AirDrag = 0.5f;           // Slight drag over time

    internal Vector3 Direction;

    public override void PhysicsProcess(float delta)
    {
        Direction = _player.Input.Direction;

        Vector3 currentHorz = new Vector3(_velocity.X, 0, _velocity.Z);
        float currentSpeed = currentHorz.Length();

        if (Direction != Vector3.Zero)
        {
            Vector3 wishDir = Direction.Normalized();

            // Project current velocity onto wish direction to measure aligned speed
            float currentSpeedInWishDir = currentHorz.Dot(wishDir);

            // Calculate remaining speed headroom up to AirSpeed
            float addSpeed = AirSpeed - currentSpeedInWishDir;

            if (addSpeed > 0)
            {
                // Calculate acceleration force for this frame
                float accelSpeed = AirAcceleration * delta * AirSpeed;
                accelSpeed = Mathf.Min(accelSpeed, addSpeed);

                // Add force in the wish direction
                currentHorz += wishDir * accelSpeed;
            }
        }

        // Apply slight air drag to prevent permanent infinite coasting
        if (AirDrag > 0f)
        {
            currentHorz = currentHorz.MoveToward(Vector3.Zero, AirDrag * delta);
        }

        // Apply updated horizontal velocity and gravity
        SetVelocity(new Vector3(currentHorz.X, _velocity.Y, currentHorz.Z));
        AddVelocity(_player.GetGravity() * delta);
    }

    public override void Enter(State previous = null)
    {
        base.Enter(previous);
        _player.Camera.SetBobVariables(0f, 0f);
    }
}