using Godot;
using Godot.Collections;
using System.Linq;


[GlobalClass]
public partial class FacilityManager : Node
{
    public int NumFacilities => _facilities.Count;

    private Array<FacilityBase> _facilities;

    public override void _Ready()
    {
        _facilities = new();
        foreach (Node child in GetChildren())
        {
            if (child is not FacilityBase facility) continue;
            _facilities.Add(facility);
        }
    }

    public bool TryGetRandom(out FacilityBase facility, Array<FacilityBase> exclude = null)
    {
        facility = default;
        if (exclude == null)
        {
            facility = _facilities.PickRandom();
            return true;
        }
        Array<FacilityBase> tmp = [.. _facilities.Except(exclude)];
        if (tmp.Count > 0)
        {
            facility = tmp.PickRandom();
            return true;
        }
        return false;
    }
}