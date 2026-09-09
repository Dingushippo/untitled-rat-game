using Godot;
using System;

public partial class HurtboxComponent : Area3D, IDamageable
{
    [Export] public bool IsEntity;
    [Export] public bool IsWeakspot;
    [Export] public float WeakSpotMultiplier = 1f;
    public event Action<float> OnHit;
    public override void _Ready()
    {
        if (IsEntity)
        {
            CollisionLayer = PhysicsLayers.ENTITY_HURTBOX;
            CollisionMask = PhysicsLayers.PLAYER_HITBOX;
        }
        else
        {
            CollisionLayer = PhysicsLayers.PLAYER_HURTBOX;
            CollisionMask = PhysicsLayers.ENTITY_HITBOX;
        }
    }

    public void TakeDamage(float amount)
    {
        float damage = IsWeakspot ? amount * WeakSpotMultiplier : amount;
        OnHit?.Invoke(amount);
    }

}
