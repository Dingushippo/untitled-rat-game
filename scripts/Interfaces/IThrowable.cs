
using Godot;

public interface IThrowable
{
    void Throw(Vector3 direction, float force);
    void Throw(Vector3 direction, Vector3 position);
}