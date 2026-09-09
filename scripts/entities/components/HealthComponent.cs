using Godot;
using System;


[GlobalClass]
public partial class HealthComponent : Node
{
    [Export] public float MaxHealth;

    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }
    private float _health;

    public event Action<float, bool> OnDamage;
    public event Action<float> OnHeal;
    public event Action OnDeath;

    public override void _Ready()
    {
        Health = MaxHealth;
    }

    public void Damage(float amount, bool isWeakspot)
    {
        Health -= amount;
        OnDamage?.Invoke(amount, isWeakspot);

        if (Health == 0)
            OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        Health += amount;
        OnHeal?.Invoke(amount);

        if (Health == 0)
            OnDeath?.Invoke();
    }
}