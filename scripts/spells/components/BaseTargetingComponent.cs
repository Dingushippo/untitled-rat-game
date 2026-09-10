using Godot;

[GlobalClass]
public abstract partial class BaseTargetingComponent : SpellComponent
{
    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);
        AcquireTarget();
        RaiseComplete(_payload);
    }
    protected abstract void AcquireTarget();
}