using Godot;

public class PlayerVaultState : PlayerState
{
    public PlayerVaultState(Player owner) : base(owner) { }

    private Vector3 vaultPoint;
    private Vector3 startPoint;
    private Vector3 midPoint;
    public override void Enter(State previous = null)
    {
        Vector3 forwardDir = -_player.GlobalBasis.Z;
        vaultPoint = _player.VaultRaycast.GetCollisionPoint() + forwardDir * 0.1f;
        startPoint = _player.GlobalPosition;
        midPoint = startPoint.Lerp(vaultPoint, 0.5f) + new Vector3(0, 0.5f, 0);

        Tween vaultTween = _player.CreateTween();
        vaultTween.SetEase(Tween.EaseType.InOut);
        vaultTween.SetTrans(Tween.TransitionType.Sine);
        vaultTween.TweenMethod(Callable.From<float>(BezierMove), 0f, 1f, 0.4f);
        vaultTween.TweenCallback(Callable.From(() => fsm.ChangeState("idle", this)));
    }

    private void BezierMove(float t)
    {
        Vector3 a = startPoint.Lerp(midPoint, t);
        Vector3 b = midPoint.Lerp(vaultPoint, t);
        _player.GlobalPosition = a.Lerp(b, t);
    }

    public override void Exit()
    {
        Vector3 velocity = _player.Velocity;
        velocity.Y = 0;
        _player.Velocity = velocity;
    }
}