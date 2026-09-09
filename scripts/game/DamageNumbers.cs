using Godot;
using System;


[GlobalClass]
public partial class DamageNumbers : Node
{
    private const float DECAY = 0.5f;
    private const float VERTICAL_MOVEMENT = 0.5f;
    private const float HORIZONTAL_RANGE = 1f;
    private readonly Color _baseColor = Colors.White;
    private readonly Color _critColor = Colors.Red;

    private static DamageNumbers _instance;
    public static DamageNumbers Instance => _instance;
    private ObjectPoolComponent _objectPool;


    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this))
            return;

        _objectPool = ObjectPoolComponent.FromType<PooledLabel3D>(this, poolSize: 100);
        GD.Print($"object pool: {_objectPool}");
    }

    public void Spawn(Vector3 position, float value, bool isWeakspot = false)
    {
        if (_objectPool.TrySpawnObject(out PooledLabel3D label, position))
        {
            Color color = isWeakspot ? _critColor : _baseColor;
            label.Modulate = color;
            label.Text = value.ToString();
            TweenLabel(label);
        }
    }

    private void TweenLabel(PooledLabel3D label)
    {
        float yPos = label.GlobalPosition.Y + VERTICAL_MOVEMENT;
        float xPos = label.GlobalPosition.X + (float)GD.RandRange(-HORIZONTAL_RANGE, HORIZONTAL_RANGE);

        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(label, "modulate:a", 0, DECAY);
        tween.TweenProperty(label, "global_position:y", yPos, DECAY);
        tween.TweenProperty(label, "global_position:x", xPos, DECAY);
        tween.Chain();
        tween.TweenCallback(Callable.From(() => _objectPool.DespawnObject(label)));
    }
}

public partial class PooledLabel3D : Label3D, IPooledObject
{
    public override void _Ready()
    {
        Billboard = BaseMaterial3D.BillboardModeEnum.FixedY;
        FontSize = 96;
    }
    public void OnDespawn()
    {
        Hide();
        Text = "";
    }

    public void OnSpawn()
    {
        Show();
        Color newColor = Modulate;
        newColor.A = 1f;
        Modulate = newColor;
    }
}