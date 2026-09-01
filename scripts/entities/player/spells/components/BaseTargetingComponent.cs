using Godot;

[GlobalClass]
public abstract partial class BaseTargetingComponent : SpellComponent
{
    public SpellPayload Payload { get; set; }
    protected Node3D _spellOwner;

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        AcquireTarget();
        RaiseComplete(payload);
    }
    protected abstract void AcquireTarget();
}