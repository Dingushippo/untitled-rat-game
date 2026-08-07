using Godot;
using System;


[GlobalClass, Tool]
public partial class RitualRenderer : Node2D
{
	[Export] public RitualCircleResource Ritual { get; set; }
    [Export] public float LineThickness = 2f;

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

	public override void _Draw()
	{
		if (Ritual == null)
			return;

		foreach (RitualElement element in Ritual.Elements)
		{
			if (!element.Visible || element is null)
				continue;

			DrawElement(element);
		}
	}

	private void DrawElement(RitualElement element)
	{
		switch (element)
		{
			case RingElement ring:
				DrawRing(ring);
				break;

			case LineElement line:
				DrawLine(line);
				break;

			case RuneElement rune:
				DrawRune(rune);
				break;
		}
	}

    private void DrawRing(RingElement element)
    {
        DrawCircle(element.Position, element.Radius, Colors.White, false, LineThickness);
    }

    private void DrawLine(LineElement element)
    {
        DrawLine(element.Position, element.EndPosition, Colors.White, LineThickness);
    }

    private void DrawRune(RuneElement element)
    {
        throw new NotImplementedException();
    }
}