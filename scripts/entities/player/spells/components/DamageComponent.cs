using Godot;

[GlobalClass]
public partial class DamageComponent : SpellComponent
{
    [Export] public float Amount;

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);
        foreach (Node3D node in payload.TargetNodes)
        {
            if (node is IDamageable damageable)
            {
                damageable.TakeDamage(Amount);
            }
        }
        RaiseComplete(_payload);
    }
}