using Godot;
using System;

public partial class NavigationRegion3d : NavigationRegion3D
{
    public override void _Ready()
    {
        EventBus.Subscribe(Event.ObjectPlaced, OnObjectPlaced);
    }
    private void OnObjectPlaced(object[] args)
    {
        BakeNavigationMesh();
    }
}
