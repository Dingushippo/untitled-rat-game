using Godot;

[GlobalClass]
public partial class DamageComponent : SpellComponent
{
    [Export] public float DamageAmount = 10f;

    public override void Execute(SpellPayload payload)
    {
        foreach (Node3D node in payload.TargetNodes)
            if (node is IDamageable victim)
            {
                victim.TakeDamage(DamageAmount);
            }
    }
}