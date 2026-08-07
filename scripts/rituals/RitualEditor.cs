using Godot;

public partial class RitualEditor : Control
{
	[Export] public RitualCanvas Canvas { get; set; }

	[Export] public RitualRenderer Renderer { get; set; }

	[Export] public RitualSelection Selection { get; set; }

	private RitualTool _activeTool;

	public RitualCircleResource Ritual { get; private set; }

	public override void _Ready()
	{
		NewRitual();
		SetTool(new RingTool(this));
	}

	public void NewRitual()
	{
		Ritual = new RitualCircleResource();

		Renderer.Ritual = Ritual;
		Renderer.QueueRedraw();
	}

	public void AddElement(RitualElement element)
	{
		Ritual.Elements.Add(element);
		Renderer.QueueRedraw();
	}

    public void SetTool(RitualTool tool)
    {
        _activeTool = tool;
        GD.Print($"Tool set: {tool}");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouse mouse)
            return;

        if (mouse is InputEventMouseMotion motion)
        {
            _activeTool.MouseMove(Canvas.ScreenToRitual(motion.Position));
        }
        else if (mouse is InputEventMouseButton button &&
                button.ButtonIndex == MouseButton.Left)
        {
            Vector2 position = Canvas.ScreenToRitual(button.Position);

            if (button.IsReleased())
                _activeTool.MouseUp(position);

            if (button.IsPressed())
                _activeTool.MouseDown(position);
        }
    }

}