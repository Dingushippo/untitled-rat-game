using Godot;

public partial class PlayerResourceComponent : Node
{
    [Export] public float MaxHealth;
    [Export] public float MaxRatEssence;

    private float _health;
    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            EventBus.Publish<PlayerHealthChanged>(new(_health, MaxHealth));
        }
    }

    private float _ratEssence;
    public float RatEssence
    {
        get => _ratEssence;
        set
        {
            _ratEssence = Mathf.Clamp(value, 0, MaxRatEssence);
            EventBus.Publish<PlayerHealthChanged>(new(_ratEssence, MaxRatEssence));
        }
    }
}

public record struct PlayerHealthChanged(float current, float max);
public record struct PlayerRatEssenceChanged(float current, float max);
