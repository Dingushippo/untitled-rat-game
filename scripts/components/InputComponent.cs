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

	public override void _Input(InputEvent @event)
	{
		// Publish mouse click inputs to the EventBus
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed)
			{	
				EventBus.Publish(new MouseClickEvent((int)mouseEvent.ButtonIndex, mouseEvent.Position));
			}
		}
	}
}

public class MouseClickEvent
{
	public int ButtonIndex { get; }
	public Vector2 Position { get; }

	public MouseClickEvent(int buttonIndex, Vector2 position)
	{
		ButtonIndex = buttonIndex;
		Position = position;
	}
}
