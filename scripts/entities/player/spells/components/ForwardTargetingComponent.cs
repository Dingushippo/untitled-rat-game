using Godot;

[GlobalClass]
public partial class ForwardTargetingComponent : BaseTargetingComponent
{
    public new string ComponentName { get; } = "Forward targeting component";
    protected override void AcquireTarget()
    {
        Vector3 origin = Payload.Caster.GlobalPosition;
        Vector3 direction = -Payload.Caster.GlobalBasis.Z;
        Payload.TargetPosition = origin + direction;
    }
}