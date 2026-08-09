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

    public Node SpawnObject(Vector3 position, Vector3 rotation = new())
    {
        Node obj = _pool.Dequeue();
        PrepareObject(obj, position, rotation);
        Active.Add(obj);
        return obj;
    }

    public T SpawnObject<T>(Vector3 position, Vector3 rotation = new()) where T : Node
    {
        return (T)SpawnObject(position, rotation);
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
}