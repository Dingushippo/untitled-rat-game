using Godot;
using System;
using System.Collections.Generic;

public partial class ObjectManager : Node
{
    private static ObjectManager _instance;
    public static ObjectManager Instance => _instance;

    [Export] public PackedScene objectScene;
    private ObjectPoolComponent _pool;
    private NavigationRegion3D _mainNavigationRegion;

    public override void _EnterTree()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this)) return;
    }
    public override void _Ready()
    {
        EventBus.Subscribe<NavigationRegionReady>(OnNavigationRegionReady);
    }

    public void SpawnObject(ObjectResource resource, Vector3 position, Vector3 rotation)
    {
        PlaceableObject obj = (PlaceableObject)_pool.SpawnObject(position, rotation);
        obj.objectResource = resource;
        EventBus.Publish(new ObjectPlaced());
    }

    public void OnNavigationRegionReady(NavigationRegionReady evt)
    {
        Node poolTarget = evt.Region.GetNode("Objects");
        _pool = new ObjectPoolComponent(poolTarget, objectScene, 10);
    }

}