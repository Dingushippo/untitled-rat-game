using Godot;

public partial class PlayerVaultState : PlayerState
{
    private Vector3 _vaultPoint;
    private Vector3 _startPoint;
    private Vector3 _midPoint;

    public override void Enter(State previous = null)
    {
        Vector3 forwardDir = -_player.GlobalBasis.Z;
        _vaultPoint = _player.VaultRaycast.GetCollisionPoint() + forwardDir * 0.1f;
        _startPoint = _player.GlobalPosition;
        _midPoint = _startPoint.Lerp(_vaultPoint, 0.5f) + new Vector3(0, 0.5f, 0);

        Tween vaultTween = _player.CreateTween();
        vaultTween.SetEase(Tween.EaseType.InOut);
        vaultTween.SetTrans(Tween.TransitionType.Sine);
        vaultTween.TweenMethod(Callable.From<float>(BezierMove), 0f, 1f, 0.4f);
        vaultTween.TweenCallback(Callable.From(() => _hfsm.ChangeState<PlayerIdleState>()));
    }

    private void BezierMove(float t)
    {
        Vector3 a = _startPoint.Lerp(_midPoint, t);
        Vector3 b = _midPoint.Lerp(_vaultPoint, t);
        _player.GlobalPosition = a.Lerp(b, t);
    }

    public override void Exit()
    {
        Vector3 velocity = _player.Velocity;
        velocity.Y = 0;
        _player.Velocity = velocity;
    }
}
