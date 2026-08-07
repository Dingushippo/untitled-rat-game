using Godot;

[Tool]
public partial class RitualCanvas : Node2D
{
	private RitualCircleResource _ritual;
	[Export] public RitualCircleResource Ritual
    {
        get => _ritual;
		set  
		{
            _ritual = value;
			if (Renderer is not null)
				Renderer.Ritual = _ritual;
        }
    }
	[Export] public RitualRenderer Renderer;

    public override void _Ready()
    {
        Renderer.Ritual = Ritual;
    }

	public Vector2 ScreenToRitual(Vector2 screenPosition)
	{
		return ToLocal(screenPosition);
	}
}