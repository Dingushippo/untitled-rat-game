using Godot;
using System;

public partial class Rat : CharacterBody3D, IGrabbable
{
	[Export] public PathfindingComponent PathfindingComponent;
	[Export] public InteractComponent InteractComponent;
	[Export] public CollisionShape3D Collider;
	[Export] bool debug = true;

	private Node3D _navigationTarget;
	private Node3D _navigationTargetOriginal;
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
		InteractComponent.OnInteract += OnInteract;
		CallDeferred(nameof(InitializeNavigationTarget));
	}

	private void OnInteract()
	{
		Grab();
	}

	public void Grab()
	{
		PathfindingComponent.DisablePathfinding();
		Collider.Disabled = true;
		_player.InitiateGrabCycle(this);
	}

	public void Release()
	{
		PathfindingComponent.EnablePathfinding();
		Collider.Disabled = false;
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
		};
		_navigationTargetOriginal = _navigationTarget;

		parent.AddChild(_navigationTarget);
		_navigationTarget.GlobalPosition = GlobalPosition;
		_navigationTarget.Owner = parent;
		PathfindingComponent.NavigationTarget = _navigationTarget;

		GD.Print("NavigationTarget created and added to the scene tree.");
	}

	private void DebugSetNavigationTargetToPlayer()
	{
		GD.Print($"Current NavigationTarget: {PathfindingComponent.NavigationTarget?.Name}");
		if (_player != null && PathfindingComponent != null && PathfindingComponent.NavigationTarget != _player)
		{
			GD.Print("Setting NavigationTarget position to Player position.");
			PathfindingComponent.NavigationTarget = _player;
		}
		else if (PathfindingComponent.NavigationTarget == _player)
		{
			GD.Print("Resetting NavigationTarget position to original position.");
			PathfindingComponent.NavigationTarget = _navigationTargetOriginal;
		}
		else
		{
			GD.Print("Player or PathfindingComponent is null.");
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
