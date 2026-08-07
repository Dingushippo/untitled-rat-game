using Godot;
using System;

[GlobalClass, Tool]
public partial class RitualRenderer : Node2D
{


    [Export] public RitualResource RitualResource;
    [Export] public float LineThickness = 1f;
    [ExportToolButton("Redraw")] public Callable RedrawButton => Callable.From(() => QueueRedraw());

    private Vector2 _center = Vector2.Zero;

    public override void _Ready()
    {
        RitualResource.Changed += QueueRedraw;
    }

    public override void _Draw()
    {
        foreach (RitualCircleResource circle in RitualResource.ritualCircles)
        {
            DrawRitualCircle(circle);
        }
    }

    private void DrawRitualCircle(RitualCircleResource circle)
    {
        float angleChange = Mathf.Tau / circle.NumElements;
        for (int i = 0; i < circle.NumElements; i++)
        {
            DrawInterpolatedArcs(circle, i, angleChange);
            DrawElementCircles(circle, i, angleChange);
        }
    }

    private void DrawInterpolatedArcs(RitualCircleResource circle, int index, float angleChange, int points = 10)
    {
        float elementAngleOffset = Mathf.Atan2(circle.ElementRadius, circle.Radius);
        float startAngle = index * angleChange + circle.AngleOffset + elementAngleOffset;
        float endAngle = index * angleChange + (angleChange - elementAngleOffset) + circle.AngleOffset;
        DrawArc(_center, circle.Radius, startAngle, endAngle, points, Colors.White, LineThickness);
    }

    private void DrawElementCircles(RitualCircleResource circle, int index, float angleChange)
    {
        Vector2 pos = new Vector2(
            circle.Radius * Mathf.Cos(angleChange * index + circle.AngleOffset),
            circle.Radius * Mathf.Sin(angleChange * index + circle.AngleOffset)
        );
        DrawCircle(_center + pos, circle.ElementRadius, Colors.White, false, LineThickness);
    }
}
