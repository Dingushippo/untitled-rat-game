using Godot;
using System;
using System.Collections.Generic;

public partial class ObjectManager : Node
{
    public static ObjectManager Instance { get; private set; }

    [Export] public PackedScene objectScene;
    private ObjectPoolComponent _pool;
    private NavigationRegion3D _mainNavigationRegion;

    public override void _EnterTree()
    {
        if (Instance == null) Instance = this;
        else Instance.QueueFree();
    }
    public override void _Ready()
    {        
        EventBus.Subscribe(Event.NavigationRegionReady, OnNavigationRegionReady);
    }

    public void SpawnObject(ObjectResource resource, Vector3 position, Vector3 rotation)
    {
        PlaceableObject obj = (PlaceableObject)_pool.SpawnObject(position, rotation);
        obj.objectResource = resource;
        EventBus.Publish(Event.ObjectPlaced);
    }

    public void OnNavigationRegionReady(object[] args)
    {
        _mainNavigationRegion = (NavigationRegion3D)args[0];
        Node poolTarget = _mainNavigationRegion.GetNode("Objects");
        _pool = new ObjectPoolComponent(poolTarget, objectScene, 10);
        GD.Print($"NavigationRegion: {_mainNavigationRegion}");
    }

}