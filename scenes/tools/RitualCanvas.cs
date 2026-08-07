using Godot;

[Tool]
public partial class RitualCanvas : Node2D
{
	public override void _Draw()
	{
		DrawCircle(
			new Vector2(512/2, 512/2),
			200,
			Colors.White,
			false,
			4
		);
	}
}