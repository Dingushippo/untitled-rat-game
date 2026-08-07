using Godot;

public abstract class RitualTool
{
	protected RitualEditor Editor { get; }

	protected RitualTool(RitualEditor editor)
	{
		Editor = editor;
	}

	public virtual void MouseDown(Vector2 position) { }
	public virtual void MouseMove(Vector2 position) { }
	public virtual void MouseUp(Vector2 position) { }

	public virtual void DrawOverlay(CanvasItem canvas) { }
}