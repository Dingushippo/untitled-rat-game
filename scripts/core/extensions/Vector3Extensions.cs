using Godot;

public static class Vector3Extensions
{
    public static Vector2 ToXYVector2(this Vector3 vector)
    {
        return new(vector.X, vector.Z);
    }
}
