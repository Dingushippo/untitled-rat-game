using Godot;
using System;


[GlobalClass]
public partial class RitualElementSlot : Node3D, ICatchArea, IPooledObject
{
    public const float COLLIDER_MARGIN = 0.1f;
    [Export] public CollisionShape3D CatchAreaCollider;
    [Export] public WorkSlot WorkSlot;

    private RitualCircleResource _ritualCircle;
    [Export]
    public RitualCircleResource RitualCircle
    {
        get => _ritualCircle;
        set
        {
            _ritualCircle = value;
            if (CatchAreaCollider.Shape is SphereShape3D shape)
            {
                shape.Radius = (RitualCircle.ElementRadius / 100) + COLLIDER_MARGIN;
            }
        }
    }
    [Export] public RitualElement Element;

    public Action Fulfilled;
    public float ColliderTopY { get; set; } = 0.1f;
    private Inventory _inventory;

    public void SetElement(RitualElement element)
    {
        Element = element;
        if (element is RitualItemElement itemElement)
        {
            _inventory = new(itemElement.Amount, [itemElement.Item.Id]);
        }
    }

    public void OnSpawn()
    {
        CatchAreaCollider.Disabled = false;
        WorkSlot.Entered += () => Fulfilled?.Invoke();
    }

    public void OnDespawn()
    {
        CatchAreaCollider.Disabled = true;
        WorkSlot.Entered -= () => Fulfilled?.Invoke();
    }

    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = ThrowTarget.Slot(WorkSlot);
        if (WorkSlot.IsOccupied) return false;
        if (Element is RitualItemElement && !rat.Cargo.HasAnythingFor(_inventory))
        {
            // TODO Find out if we are removing from inventory here, or at a later stage
            return false;
        }
        return true;
    }
}
