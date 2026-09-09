using Godot;

public partial class TargetDummy : CharacterBody3D
{
    [Export] public HurtboxComponent HurtboxHead;
    [Export] public HurtboxComponent HurtboxBody;
    [Export] public HealthComponent HealthComponent;

    public override void _Ready()
    {
        base._Ready();
        HurtboxBody.OnHit += HealthComponent.Damage;
        HurtboxHead.OnHit += HealthComponent.Damage;
        HealthComponent.OnDamage += OnHit;
        HealthComponent.OnDeath += OnDeath;
    }

    private void OnHit(float amount)
    {
        GD.Print($"Hit: {HealthComponent.Health}/{HealthComponent.MaxHealth}");
    }

    private void OnDeath()
    {
        GD.Print($"Dead");
    }

    public override void _ExitTree()
    {
        HurtboxBody.OnHit -= HealthComponent.Damage;
        HurtboxHead.OnHit -= HealthComponent.Damage;
        HealthComponent.OnDamage -= OnHit;
        HealthComponent.OnDeath -= OnDeath;
    }

}
