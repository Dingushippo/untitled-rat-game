using Godot;
using System;


// [GlobalClass, Tool]
public partial class RitualEditorViewport : SubViewportContainer
{
    private const float MIN_ZOOM = 0.1f;
    private const float MAX_ZOOM = 10f;

    [Export] public Node2D RitualCanvas;
    [Export] public SubViewport Viewport;
    private float _zoom = 1f;
    [Export(PropertyHint.Range, "0.1,10,0.1")]
    public float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = Mathf.Clamp(value, MIN_ZOOM, MAX_ZOOM);
            SetZoom(_zoom);
        }
    }


    private bool _isPanning;
    private Vector2 _pan = Vector2.Zero;
    public override void _Ready()
    {
        // Shift the canvas, so that 0, 0 is at the center
        RitualCanvas.Position = Size / 2;
    }
    public void SetZoom(float zoom)
    {
        GD.Print($"Setting zoom to: {zoom}");
        RitualCanvas.Scale = Vector2.One * zoom;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn)
        {
            if (btn.ButtonIndex == MouseButton.WheelUp) Zoom -= 0.1f;
            if (btn.ButtonIndex == MouseButton.WheelDown) Zoom += 0.1f;
            if (btn.ButtonIndex == MouseButton.Middle && btn.IsPressed()) _isPanning = true;
            if (btn.ButtonIndex == MouseButton.Middle && btn.IsReleased()) _isPanning = false;
            if (btn.ButtonIndex == MouseButton.Middle && btn.DoubleClick)
            {
                RitualCanvas.Position = Size / 2;
                Zoom = 1f;
            }
        }
        if (@event is InputEventMouseMotion motion && _isPanning)
        {
            RitualCanvas.Position += motion.Relative;
        }
    }

}
