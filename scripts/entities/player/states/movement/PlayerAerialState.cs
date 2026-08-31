using Godot;

public partial class PlayerAerialState : PlayerState
{
    [Export] public float AirSpeed = 7.0f;          // Target speed applied by WASD inputs
    [Export] public float AirAcceleration = 35.0f;  // Responsiveness of air control
    [Export] public float AirDrag = 0.5f;           // Slight drag over time

    internal Vector3 Direction;

    public override void PhysicsProcess(float delta)
    {
        if (CanVault())
        {
            _hfsm.ChangeState<PlayerVaultState>();
            return;
        }

        Direction = _player.InputComponent.Direction;
        Vector3 currentHorizontal = new Vector3(_velocity.X, 0, _velocity.Z);

        if (Direction != Vector3.Zero)
        {
            Vector3 wishDir = Direction.Normalized();

            // Project current velocity onto wish direction to measure aligned speed
            float currentSpeedInWishDir = currentHorizontal.Dot(wishDir);

            // Calculate remaining speed headroom up to AirSpeed
            float addSpeed = AirSpeed - currentSpeedInWishDir;

            if (addSpeed > 0)
            {
                // Calculate acceleration force for this frame
                float accelSpeed = AirAcceleration * delta * AirSpeed;
                accelSpeed = Mathf.Min(accelSpeed, addSpeed);

                // Add force in the wish direction
                currentHorizontal += wishDir * accelSpeed;
            }
        }

        // Apply slight air drag to prevent permanent infinite coasting
        if (AirDrag > 0f)
        {
            currentHorizontal = currentHorizontal.MoveToward(Vector3.Zero, AirDrag * delta);
        }

        // Apply updated horizontal velocity and gravity
        SetVelocity(new Vector3(currentHorizontal.X, _velocity.Y, currentHorizontal.Z));
        AddVelocity(_player.GetGravity() * delta);
    }

    public override void Enter(State previous = null)
    {
        base.Enter(previous);
        _player.Camera.SetBobVariables(0f, 0f);
    }

    private bool CanVault()
    {
        if (_player.InputComponent.WantsVault && _player.VaultRaycast.IsColliding())
            return true;
        return false;
    }
}