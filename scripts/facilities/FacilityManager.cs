using Godot;
using Godot.Collections;
using System;


[GlobalClass]
public partial class FacilityManager : Node
{
    public int NumFacilities => _facilities.Count;
    public FacilityBase Random => _facilities.PickRandom();

    private Array<FacilityBase> _facilities;

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is not FacilityBase facility) continue;
            _facilities.Add(facility);
        }
    }
}