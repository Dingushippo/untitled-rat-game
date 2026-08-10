using Godot;
using Godot.Collections;

public partial class RitualManagerNode : Node
{
    private const string RITUAL_BASE_UID = "uid://b2h3kmtker20h";
    private const string ELEMENT_SLOT_UID = "uid://b5buk60oogt6h";
    private static RitualManagerNode _instance;
    public static RitualManagerNode Instance => _instance;
    private ObjectPoolComponent _ritualPool;
    private ObjectPoolComponent _elementSlotPool;
    private RitualBase _currentHandling;

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this)) return;

        _elementSlotPool = new(this, GD.Load<PackedScene>(ELEMENT_SLOT_UID), 10);
        _ritualPool = new(this, GD.Load<PackedScene>(RITUAL_BASE_UID), 10);
    }

    public T InstanciateRitualPreview<T>(RitualResource resource, Vector3 position) where T : RitualBase
    {
        T ritual = _ritualPool.SpawnObject<T>(position);
        ritual.RitualResource = resource;
        return ritual;
    }

    public T PlaceRitual<T>(RitualResource resource, Vector3 position) where T : RitualBase
    {
        T ritual = _ritualPool.SpawnObject<T>(position);
        Array<RitualElementSlot> slots = new();
        foreach (RitualCircleResource circle in resource.RitualCircles)
        {
            foreach (RitualElement element in circle.RitualElements)
            {
                RitualElementSlot slot = _elementSlotPool.SpawnObject<RitualElementSlot>(position);

                slot.Position = new Vector3(
                    position.X + element.Position.X / 100f,
                    position.Y,
                    position.Z + element.Position.Y / 100f
                );
                slot.RitualCircle = circle;
                slot.LookAt(position);
                slot.SetElement(element);
                slots.Add(slot);
            }
        }
        ritual.Slots = slots;
        return ritual;
    }
}