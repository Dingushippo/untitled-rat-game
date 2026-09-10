using Godot;

[GlobalClass]
public partial class TrailComponent : TrailSpellComponent
{
    public override void _Ready() => Emitting = false;

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        Emitting = true;
        RaiseComplete(payload);
    }
}