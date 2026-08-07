using Godot;

public class RingTool : RitualTool
{
	private Vector2 _start;

	public RingTool(RitualEditor editor)
		: base(editor)
	{
	}

	public override void MouseDown(Vector2 position)
	{
		_start = position;
	}

	public override void MouseUp(Vector2 position)
	{
		float radius = _start.DistanceTo(position);

		var ring = new RingElement
		{
			Position = _start,
			Radius = radius
		};
        ring.Visible = true;

		Editor.AddElement(ring);
	}
}