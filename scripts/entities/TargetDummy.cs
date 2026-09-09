using Godot;

public partial class TargetDummy : CharacterBody3D
{
    [Export] public HurtboxComponent HurtboxHead;
    [Export] public HurtboxComponent HurtboxBody;
    [Export] public HealthComponent HealthComponent;
    [Export] public bool Debug;

    public override void _Ready()
    {
        base._Ready();
        HurtboxBody.OnHit += HealthComponent.Damage;
        HurtboxHead.OnHit += HealthComponent.Damage;
        HealthComponent.OnDamage += OnHit;
        HealthComponent.OnDeath += OnDeath;
    }

    private void OnHit(float amount, bool isCrit = false)
    {
        if (Debug)
            GD.Print($"Hit for {amount}: {HealthComponent.Health}/{HealthComponent.MaxHealth}");
        DamageNumbers.Instance.Spawn(GlobalPosition + Vector3.Up * 2.5f, amount, isCrit);
    }

    private void OnDeath()
    {
        if (Debug)
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
