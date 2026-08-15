using Godot;
using System;

[GlobalClass]
public partial class WorkSlot : Marker3D
{
    public Rat Occupant;
    public bool IsOccupied;
    public bool IsEntered;
    public FacilityBase Facility;
    public Action Entered;
    public Action Exited;

    public override void _Ready()
    {
        if (GetOwner() is FacilityBase facility)
            Facility = facility;
    }

    public bool TryReserve(Rat rat)
    {
        if (IsOccupied) return false;

        Occupant = rat;
        IsOccupied = true;

        return true;
    }

    public void HasEntered()
    {
        IsEntered = true;
        Entered?.Invoke();
    }

    public void Release()
    {
        Occupant = null;
        IsEntered = false;
        IsOccupied = false;
        Exited?.Invoke();
    }

}