using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class RitualViewportController : SubViewport
{
    private const float MIN_ZOOM = 0.1f;
    private const float MAX_ZOOM = 10f;

    [Export] public Node2D Canvas;
    public float Zoom {get; private set;} = 1.0f;
    private Vector2 _pan;
    
    public void SetZoom(float zoom)
    {
        Zoom = Math.Clamp(zoom, MIN_ZOOM, MAX_ZOOM);
        Canvas.Scale = Vector2.One * Zoom;
    }

    public void Pan(Vector2 amount)
    {
        _pan += amount;
        Canvas.Position = _pan;
    }

}
