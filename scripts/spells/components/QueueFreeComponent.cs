using Godot;


[GlobalClass]
public partial class QueueFreeComponent : SpellComponent
{
    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);
        spell.QueueFree();
    }
}