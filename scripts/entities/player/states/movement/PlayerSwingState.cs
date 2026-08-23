using Godot;

public class PlayerSwingState : PlayerState
{
    public PlayerSwingState(Player owner)
        : base(owner) { }

    public override void PhysicsProcess(float delta)
    {
        // Vector2 inputDir = _player.GetInputVector();
        // Transform3D cameraTransform = _player.Camera.GlobalTransform;
        // Vector3 moveDirection = (cameraTransform.Basis.X * inputDir.X).Normalized();

        // _player.Direction = moveDirection;
        // _player.HorizontalSpeed = 10f;
    }

    public override void Enter(State previous = null)
    {
        // _player.Freeze = true;
        // _player.ApplyCentralImpulse(Vector3.Forward * 5f);
        // _player.LockRotation = false;
    }

    public override void Exit()
    {
        // _player.Freeze = true;

        // // Tween rotation back to normal
        // _startBasis = _player.GlobalBasis;
        // Tween tween = _player.CreateTween();
        // tween.TweenMethod(Callable.From((float w) => InterpolateBasis(w)), 0.0, 1.0f, 0.3f);
        // tween.TweenCallback(Callable.From(() => ResetRigidbody()));
    }

    Basis _startBasis;

    private void InterpolateBasis(float weight)
    {
        _player.GlobalBasis = _startBasis.Slerp(Basis.Identity, weight);
    }

    private void ResetRigidbody()
    {
        _player.Freeze = false;
        _player.LockRotation = true;
    }
}
