using Godot;
using System;

public partial class InputComponent : Node
{
	public Vector2 Direction { get; private set; }
	public bool IsJumping { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Direction = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		IsJumping = Input.IsActionPressed("jump");
	}
}
