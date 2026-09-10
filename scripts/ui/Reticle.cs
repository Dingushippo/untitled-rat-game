using Godot;
using Godot.Collections;
using System.Net;

public partial class Reticle : Control
{
    [Export] public int N = 5;
    [Export] public float ReticleRadius = 4f;
    [Export] public float LineSpacing = 1f;
    [Export] public float LineWidth = 1f;
    [Export] public float AngleOffset = 0f;


    private float _reticleRadius = 4f;
    private Color _reticleBaseColor = Colors.White;

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawSpacedPolygon();
    }

    private void DrawSpacedPolygon()
    {
        float angleBase = Mathf.Tau / N;
        Array<Vector2> centers = [];
        for (int i = 0; i < N; i++)
        {
            float angle = angleBase * i + AngleOffset;
            centers.Add(new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * ReticleRadius);
        }

        for (int i = 0; i < N; i++)
        {
            Vector2 p1 = centers[(i - 1 + N) % N];
            Vector2 p2 = centers[i];
            Vector2 p3 = centers[(i + 1) % N];
            DrawPolyline([
                p2 + p2.DirectionTo(p1) * LineSpacing,
                p2,
                p2 + p2.DirectionTo(p3) * LineSpacing
            ], _reticleBaseColor, LineWidth, false);
        }
    }

}
