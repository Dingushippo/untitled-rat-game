using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public InputComponent InputComponent;
	[Export] public MovementComponent MovementComponent;
	[Export] public HandController Hand;

	public override void _Ready()
	{
		if (InputComponent == null)
		{
			GD.PrintErr("Player requires an InputComponent to function.");
		}
		if (MovementComponent == null)
		{
			GD.PrintErr("Player requires a MovementComponent to function.");
		}
		if (Hand == null)
		{
			GD.PrintErr("Player requires a Hand to perform grab actions.");
		}
	}
	public override void _Process(double delta)
	{
		if (InputComponent == null || MovementComponent == null) return;

		MovementComponent.Direction = InputComponent.Direction;
		MovementComponent.IsJumping = InputComponent.IsJumping;
	}

	public void InitiateGrabCycle(Node3D target)
	{
		Hand.GrabCycle(target);
	}
}
