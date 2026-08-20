using System.Collections.Generic;
using Godot;

public partial class RatManager : Node
{
    [Export]
    public Player Player;

    [Export]
    public PackedScene RatScene;
    private static RatManager _instance;
    public static RatManager Instance => _instance;

    private ObjectPoolComponent _ratPool;

    public bool CanSpawnRats(int num = 1) => _ratPool.NumAvailable >= num;

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this))
            return;

        _ratPool = new(this, RatScene, 500);
    }

    public void Despawn(Rat rat)
    {
        _ratPool.DespawnObject(rat);
    }

    public void Despawn(IEnumerable<Rat> rats)
    {
        foreach (Rat rat in rats)
        {
            Despawn(rat);
        }
    }

    public bool TrySpawnRat(out Rat rat, Vector3 position)
    {
        if (_ratPool.TrySpawnObject(out rat, position))
        {
            return true;
        }
        return false;
    }
}
