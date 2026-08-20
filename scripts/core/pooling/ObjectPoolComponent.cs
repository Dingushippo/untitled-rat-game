using System.Collections.Generic;
using Godot;

public partial class ObjectPoolComponent
{
    public List<Node> Active = new();
    private readonly Queue<Node> _pool = new Queue<Node>();
    private int _poolSize;
    private PackedScene _scene;
    private Node _parentNode;

    public ObjectPoolComponent(Node parentNode, PackedScene scene, int poolSize = 100)
    {
        _parentNode = parentNode;
        _scene = scene;
        _poolSize = poolSize;
        PreallocatePool();
    }

    public int NumAvailable => _pool.Count;

    private void PreallocatePool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            Node instance = _scene.Instantiate();
            if (instance is IPooledObject pooledObj)
            {
                _parentNode.AddChild(instance);
                pooledObj.OnDespawn(); // Initialize to dormant state
                _pool.Enqueue(instance);
            }
        }
    }

    public void PrepareObject(Node obj, Vector3 position, Vector3 rotation)
    {
        if (obj is Node3D node)
        {
            node.GlobalPosition = position;
            node.Rotation = rotation;
        }
        if (obj is IPooledObject pooled)
        {
            pooled.OnSpawn();
        }
    }

    public bool TrySpawnObject(out Node obj, Vector3 position, Vector3 rotation = new())
    {
        obj = default;
        if (_pool.Count == 0)
        {
            GD.PushError($"{_parentNode} pool is empty, cannot spawn");
            return false;
        }
        obj = _pool.Dequeue();
        PrepareObject(obj, position, rotation);
        Active.Add(obj);
        return true;
    }

    public bool TrySpawnObject<T>(out T obj, Vector3 position, Vector3 rotation = new())
        where T : Node
    {
        obj = default;
        if (TrySpawnObject(out Node obj2, position, rotation))
        {
            obj = obj2 as T;
            return true;
        }
        return false;
    }

    public void DespawnObject(Node obj)
    {
        if (obj is IPooledObject pooled)
        {
            pooled.OnDespawn();
            Active.Remove(obj);
            _pool.Enqueue(obj);
        }
    }

    public override string ToString()
    {
        return $"queue: {_pool.Count}, active: {Active.Count}";
    }
}
