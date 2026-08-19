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

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this))
            return;

        _ratPool = new(this, RatScene, 100);
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
