using Godot;
using System;


[GlobalClass]
public partial class ChargeComponent : Node, ISpellComponent
{
    [Export] public float[] ChargeLevels = [0];

    private float _chargeTimer;

    public SpellPayload Payload { get; set; }
    public Action<SpellPayload> OnComplete { get; set; }

    public void Initialize(Node3D spell, SpellPayload payload)
    {
        Payload = payload;
    }

    public void Process(float delta)
    {
        _chargeTimer += delta;

        if (Input.IsActionJustReleased("right_hand"))
        {
            int index = Array.BinarySearch(ChargeLevels, _chargeTimer);

            // If the exact value isn't found, BinarySearch returns the bitwise complement 
            // of the index of the next largest element.
            int level = index >= 0 ? index : ~index;
            Payload.SpellLevel = level;
            OnComplete?.Invoke(Payload);
        }
    }
}