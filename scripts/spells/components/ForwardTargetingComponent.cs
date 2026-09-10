using Godot;

[GlobalClass]
public partial class ForwardTargetingComponent : BaseTargetingComponent
{
    protected override void AcquireTarget()
    {
        Vector3 origin = _payload.Caster.GlobalPosition;
        Vector3 direction = -_payload.Caster.GlobalBasis.Z;
        _payload.TargetPosition = origin + direction;
    }
}