using Godot;
using System;

public partial class Rat : CharacterBody3D
{
	[Export] public MovementComponent MovementComponent;
	[Export] bool debug = true;

	private Node3D _navigationTarget;

	public override void _Ready()
	{
		if (MovementComponent == null)
		{
			GD.PrintErr("Rat requires a MovementComponent to function.");
		}

		if (debug)
		{
			EventBus.Subscribe<DebugAimMarkerEvent>(OnDebugMouseClick);
		}

		CallDeferred(nameof(InitializeNavigationTarget));
	}

	private void InitializeNavigationTarget()
	{
		if (MovementComponent == null)
		{
			return;
		}

		if (MovementComponent.NavigationTarget != null)
		{
			_navigationTarget = MovementComponent.NavigationTarget;
			return;
		}

		Node parent = GetTree().CurrentScene;
		if (parent == null)
		{
			GD.PrintErr("Unable to add NavigationTarget because the current scene is not available yet.");
			return;
		}

		_navigationTarget = new Node3D
		{
			Name = "NavigationTarget",
			GlobalPosition = GlobalPosition
		};

		parent.AddChild(_navigationTarget);
		_navigationTarget.Owner = parent;
		MovementComponent.NavigationTarget = _navigationTarget;

		GD.Print("NavigationTarget created and added to the scene tree.");
	}

	private void OnDebugMouseClick(DebugAimMarkerEvent evt)
	{
		GD.Print($"Debug Aim Marker Position: {evt.MarkerPosition}");
		if (MovementComponent != null && MovementComponent.NavigationTarget != null)
		{
			MovementComponent.NavigationTarget.GlobalPosition = evt.MarkerPosition;
		}
	}
}
