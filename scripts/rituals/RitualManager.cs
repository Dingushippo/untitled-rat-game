using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RitualManager : Node
{
    private const string RITUAL_BASE_UID = "";
    private const string ELEMENT_SLOT_UID = "";
    private RitualManager _instance;
    public RitualManager Instance => _instance;
    private ObjectPoolComponent _ritualPool;
    private ObjectPoolComponent _elementSlotPool;

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this)) return;

        _ritualPool = new(this, GD.Load<PackedScene>(RITUAL_BASE_UID), 10);
        _elementSlotPool = new(this, GD.Load<PackedScene>(ELEMENT_SLOT_UID), 100);
    }
    public T PrepareRitual<T>(RitualResource resource, Vector3 position) where T : RitualBase
    {
        T ritual = _ritualPool.SpawnObject<T>(position);
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
                slot.SetElement(element);
                ritual.Slots.Add(slot);
            }
        }
        return ritual;
    }
}