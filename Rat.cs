using Godot;
using System;

public partial class Rat : CharacterBody3D
{
	[Export] public PathfindingComponent PathfindingComponent;
	[Export] public InteractComponent InteractComponent;
	[Export] bool debug = true;

	private Node3D _navigationTarget;
	private Player _player;

	public override void _Ready()
	{
		if (PathfindingComponent == null)
		{
			GD.PrintErr("Rat requires a PathfindingComponent to function.");
		}

		if (debug)
		{
			EventBus.Subscribe<DebugAimMarkerEvent>(OnDebugMouseClick);
		}
		_player = GetTree().GetFirstNodeInGroup("player") as Player;
		InteractComponent.OnInteract += DebugSetNavigationTargetToPlayer;
		CallDeferred(nameof(InitializeNavigationTarget));
	}

	private void InitializeNavigationTarget()
	{
		if (PathfindingComponent == null)
		{
			return;
		}

		if (PathfindingComponent.NavigationTarget != null)
		{
			_navigationTarget = PathfindingComponent.NavigationTarget;
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
		PathfindingComponent.NavigationTarget = _navigationTarget;

		GD.Print("NavigationTarget created and added to the scene tree.");
	}

	private void DebugSetNavigationTargetToPlayer()
	{
		GD.Print("DebugSetNavigationTargetToPlayer called.");
		if (_player != null && PathfindingComponent != null)
		{
			GD.Print("Setting NavigationTarget position to Player position.");
			PathfindingComponent.NavigationTarget = _player;
		}
	}

	private void OnDebugMouseClick(DebugAimMarkerEvent evt)
	{
		if (PathfindingComponent != null && PathfindingComponent.NavigationTarget != null )
		{
			GD.Print($"Setting NavigationTarget position to {evt.MarkerPosition}");
			PathfindingComponent.NavigationTarget.GlobalPosition = evt.MarkerPosition;
		}
	}
}
