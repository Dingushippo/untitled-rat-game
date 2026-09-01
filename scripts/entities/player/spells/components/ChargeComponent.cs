using Godot;
using System;


[GlobalClass]
public partial class ChargeComponent : SpellComponent
{
    [Export] public float[] ChargeLevels = [0];

    private float _chargeTimer;

    public override void Process(float delta)
    {
        _chargeTimer += delta;

        Vector3 origin = _payload.Caster.GlobalPosition;
        Vector3 direction = -_payload.Caster.GlobalBasis.Z;

        _spell.GlobalPosition = origin;

        if (direction.LengthSquared() > 0.001)
            _spell.LookAt(origin + direction);

        if (Input.IsActionJustReleased("right_hand"))
        {
            int index = Array.BinarySearch(ChargeLevels, _chargeTimer);
            int level = index >= 0 ? index : ~index;

            _payload.SpellLevel = level;
            RaiseComplete(_payload);
        }
    }
}