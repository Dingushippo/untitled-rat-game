using Godot;

public sealed class SelectTool : RitualTool
{
	private RitualElement? _draggedElement;
	private Vector2 _dragOffset;
	private bool _dragging;

	public SelectTool(RitualEditor editor)
		: base(editor)
	{
	}

	public override void MouseDown(Vector2 position)
	{
		RitualElement? element = FindElement(position);

		if (element == null)
		{
			Editor.Selection.Clear();
			return;
		}

		Editor.Selection.Select(element);

		_draggedElement = element;
		_dragOffset = element.Position - position;
		_dragging = true;

		Editor.QueueRedraw();
	}

	public override void MouseMove(Vector2 position)
	{
		if (!_dragging || _draggedElement == null)
			return;

		_draggedElement.Position = position + _dragOffset;

		Editor.QueueRedraw();
	}

	public override void MouseUp(Vector2 position)
	{
		_draggedElement = null;
		_dragging = false;
	}

	private RitualElement? FindElement(Vector2 position)
	{
		// Search backwards so the topmost element gets selected first.
		for (int i = Editor.Ritual.Elements.Count - 1; i >= 0; i--)
		{
			RitualElement element = Editor.Ritual.Elements[i];

			if (!element.Visible)
				continue;

			if (HitTest(element, position))
				return element;
		}

		return null;
	}

	private static bool HitTest(
		RitualElement element,
		Vector2 position)
	{
		switch (element)
		{
			case RingElement ring:
				return HitTestRing(ring, position);

			case LineElement line:
				return HitTestLine(line, position);

			case RuneElement rune:
				return HitTestRune(rune, position);

			default:
				return false;
		}
	}

	private static bool HitTestRing(
		RingElement ring,
		Vector2 position)
	{
		float distance = position.DistanceTo(ring.Position);

		const float hitTolerance = 10f;

		return Mathf.Abs(distance - ring.Radius) <= hitTolerance;
	}

	private static bool HitTestLine(
		LineElement line,
		Vector2 position)
	{
		Vector2 start = line.Position;
		Vector2 end = line.EndPosition;

		Vector2 closest = Geometry2D.GetClosestPointToSegment(
			position,
			start,
			end
		);

		return position.DistanceTo(closest) <= 10f;
	}

	private static bool HitTestRune(
		RuneElement rune,
		Vector2 position)
	{
		return new Rect2(
			rune.Position - Vector2.One * rune.Size / 2f,
			Vector2.One * rune.Size
		).HasPoint(position);
	}
}