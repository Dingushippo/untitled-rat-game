using Godot;

[GlobalClass]
public partial class ForwardTargetingComponent : BaseTargetingComponent
{
    public new string ComponentName { get; } = "Forward targeting component";
    protected override void AcquireTarget()
    {
        Vector3 origin = _payload.Caster.GlobalPosition;
        Vector3 direction = -_payload.Caster.GlobalBasis.Z;
        _payload.TargetPosition = origin + direction;
    }
}