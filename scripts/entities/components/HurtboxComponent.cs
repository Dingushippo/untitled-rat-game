using Godot;
using System;


[GlobalClass]
public partial class HurtboxComponent : Area3D, IDamageable
{
    [Export] public bool IsPlayer;
    [Export] public bool IsWeakspot;
    [Export] public float WeakSpotMultiplier = 1f;
    public event Action<float, bool> OnHit;
    public override void _Ready()
    {
        if (IsPlayer)
        {
            CollisionLayer = PhysicsLayers.PLAYER_HURTBOX;
            CollisionMask = PhysicsLayers.ENTITY_HITBOX;
        }
        else
        {
            CollisionLayer = PhysicsLayers.ENTITY_HURTBOX;
            CollisionMask = PhysicsLayers.PLAYER_HITBOX;
        }
    }

    public void TakeDamage(float amount)
    {
        float damage = IsWeakspot ? amount * WeakSpotMultiplier : amount;
        OnHit?.Invoke(damage, IsWeakspot);
    }

}
