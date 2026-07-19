using Godot;
using System;
using System.Collections.Generic;

public partial class ObjectManager : Node
{
    public static ObjectManager Instance { get; private set; }
    [Export] public PackedScene objectScene;
    private ObjectPoolComponent _pool;

    public override void _EnterTree()
    {
        if (Instance == null) Instance = this;
        else Instance.QueueFree();
    }
    public override void _Ready()
    {
        _pool = new ObjectPoolComponent(this, objectScene);
    }

    public void SpawnObject(ObjectResource resource, Vector3 position, Vector3 rotation)
    {
        PlaceableObject obj = (PlaceableObject)_pool.SpawnObject(position, rotation);
        obj.objectResource = resource;
    }

}