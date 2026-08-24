using Godot;

public static class VectorUtils
{
    public static Vector3[] FindCardinalPoints(Vector3 point, Vector3 normal, float radius)
    {
        Vector3 arbitrary = Mathf.Abs(normal.Dot(Vector3.Up)) < 0.99 ? Vector3.Up : Vector3.Right;
        Vector3 tangent = normal.Cross(arbitrary).Normalized();
        Vector3 bitTangent = normal.Cross(tangent).Normalized();
        return
        [
            point + (tangent * radius),
            point - (tangent * radius),
            point + (bitTangent * radius),
            point - (bitTangent * radius),
        ];
    }
}
