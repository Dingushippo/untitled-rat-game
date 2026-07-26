using Godot;
using System;

[GlobalClass]
public partial class WorkSlot : Marker3D
{
    public Rat Occupant;
    public bool IsOccupied;
    public FacilityBase Facility;

    public override void _Ready()
    {
        Facility = GetOwner<FacilityBase>();
    }


    public bool TryReserve(Rat rat)
    {
        if (IsOccupied) return false;

        Occupant = rat;
        IsOccupied = true;

        return true;
    }

    public void Release()
    {
        Occupant = null;
        IsOccupied = false;
    }

}
