using Godot;

public interface ICatchArea
{
    public float ColliderTopY { get; set; }
    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target);
}