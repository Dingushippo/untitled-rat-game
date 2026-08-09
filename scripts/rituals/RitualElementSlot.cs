using Godot;
using System;


[GlobalClass]
public partial class RitualElementSlot : Node3D, ICatchArea //, IPooledObject
{
    public const float COLLIDER_MARGIN = 1f;
    [Export] public CollisionShape3D CatchAreaCollider;
    [Export] public WorkSlot WorkSlot;
    [Export] public RitualCircleResource RitualCircle;
    [Export] public RitualElement Element;
    public bool IsActive { get; set; }

    public float ColliderTopY { get; set; } = 0.1f;
    private Inventory _inventory;

    public void SetElement(RitualElement element)
    {
        Element = element;
        if (element is RitualItemElement itemElement)
        {
            _inventory = new(itemElement.Amount, [itemElement.Item.Id]);
        }
        WorkSlot.LookAt(GetParent<Node3D>().GlobalPosition);
    }

    public void OnSpawn()
    {
        CatchAreaCollider.Disabled = false;
        CatchAreaCollider.Shape = new SphereShape3D()
        {
            Radius = RitualCircle.ElementRadius + COLLIDER_MARGIN,
        };
    }

    public void OnDespawn()
    {
        CatchAreaCollider.Disabled = true;
    }

    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = ThrowTarget.Slot(WorkSlot);
        if (Element is RitualItemElement && !rat.Cargo.HasAnythingFor(_inventory))
        {
            // TODO Find out if we are removing from inventory here, or at a later stage
            return false;
        }
        return true;
    }
}
