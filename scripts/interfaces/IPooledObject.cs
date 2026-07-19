using Godot;

public interface IPooledObject
{
    bool IsActive { get; set; }
    void OnSpawn();
    void OnDespawn();
}
