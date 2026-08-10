using Godot;
using System;
using System.Linq.Expressions;

[GlobalClass, Tool]
public partial class RitualRenderer : Node2D
{
    private RitualResource _ritualResource;
    [Export]
    public RitualResource RitualResource
    {
        get => _ritualResource;
        set
        {
            _ritualResource = value;
            QueueRedraw();
        }
    }

    public float _lineThickness = 1f;
    [Export]
    public float LineThickness
    {
        get => _lineThickness;
        set
        {
            _lineThickness = value;
            QueueRedraw();
        }
    }

    private Color _colorOverride = Colors.White;
    [Export]
    public Color ColorOverride
    {
        get => _colorOverride;
        set
        {
            _colorOverride = value;
            QueueRedraw();
        }
    }

    private Vector2 _center = Vector2.Zero;

    public override void _Draw()
    {
        if (RitualResource == null) return;
        foreach (RitualCircleResource circle in RitualResource.RitualCircles)
        {
            DrawRitualCircle(circle);
        }
    }

    private void DrawRitualCircle(RitualCircleResource circle)
    {
        if (circle.NumElements <= 0)
        {
            DrawCircle(_center, circle.Radius, Colors.White, false, LineThickness);
            return;
        }
        float angleChange = Mathf.Tau / circle.NumElements;
        for (int i = 0; i < circle.NumElements; i++)
        {
            DrawInterpolatedArcs(circle, i, angleChange);
            DrawElementCircle(circle, i);
        }
    }

    private void DrawInterpolatedArcs(RitualCircleResource circle, int index, float angleChange, int points = 10)
    {
        DrawSetTransform(_center, 0);
        float elementAngleOffset = Mathf.Atan2(circle.ElementRadius, circle.Radius);
        float startAngle = index * angleChange + circle.AngleOffset + elementAngleOffset;
        float endAngle = index * angleChange + (angleChange - elementAngleOffset) + circle.AngleOffset;
        DrawArc(Vector2.Zero, circle.Radius, startAngle, endAngle, points, ColorOverride, LineThickness);
    }

    private void DrawElementCircle(RitualCircleResource circle, int index)
    {
        RitualElement element = circle.RitualElements[index];
        DrawSetTransform(element.Position, 0);
        DrawCircle(Vector2.Zero, circle.ElementRadius, ColorOverride, false, LineThickness);

        if (element.Symbol != null)
        {
            DrawSetTransform(element.Position, element.Rotation, Vector2.One * circle.SymbolScale);
            DrawTexture(element.Symbol, -new Vector2(32, 32), ColorOverride);
        }
    }
}
