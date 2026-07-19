using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public InputComponent InputComponent;
	[Export] public MovementComponent MovementComponent;
	[Export] public HandController Hand;
	[Export] public ObjectPlacementRaycast raycast;
	[Export] public ObjectResource[] DebugObjectResources;

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

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsPressed())return;

		if (@event is InputEventKey key)
        {
			ObjectResource res;
            if (key.Keycode == Key.Key1)
            {
				res = DebugObjectResources[0];
            }
			else if (key.Keycode == Key.Key2)
            {
                res = DebugObjectResources[1];
            }
			else return;

			Vector3 spawnPos = raycast.lookPoint;
			Vector3 spawnRot = GlobalRotation;

			ObjectManager.Instance.SpawnObject(res, spawnPos, spawnRot);
        }
    }

	public void InitiateGrabCycle(Node3D target)
	{
		Hand.GrabCycle(target);
	}


}
