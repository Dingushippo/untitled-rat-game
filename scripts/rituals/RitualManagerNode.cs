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

    public RitualBase InstanciateRitualPreview(RitualResource resource, Vector3 position)
    {
        RitualBase ritual = _ritualPool.SpawnObject<RitualBase>(position);
        ritual.RitualResource = resource;
        return ritual;
    }

    public void BuildElements(RitualBase ritual)
    {
        Array<RitualElementSlot> slots = new();
        foreach (RitualCircleResource circle in ritual.RitualResource.RitualCircles)
        {
            foreach (RitualElement element in circle.RitualElements)
            {
                RitualElementSlot slot = _elementSlotPool.SpawnObject<RitualElementSlot>(ritual.GlobalPosition);

                slot.Position = new Vector3(
                    ritual.GlobalPosition.X + element.Position.X / 100f,
                    ritual.GlobalPosition.Y,
                    ritual.GlobalPosition.Z + element.Position.Y / 100f
                );
                slot.RitualCircle = circle;
                slot.LookAt(ritual.GlobalPosition);
                slot.SetElement(element);
                slots.Add(slot);
            }
        }
        ritual.Slots = slots;
    }

    public void DisposeRitual(RitualBase ritual)
    {
        foreach (RitualElementSlot slot in ritual.Slots)
        {
            _elementSlotPool.DespawnObject(slot);
        }
        _ritualPool.DespawnObject(ritual);
    }
}