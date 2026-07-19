using Godot;
using System;


[GlobalClass]
public partial class HandController : Node3D
{
	[Export] public Node3D HandNode;
	[Export] public PlayerCamera camera;
	private Node3D _handTarget;
	private Node3D _oldTargetParent;
	private Vector3 _originalTargetPosition;
	private Vector3 _handOffsetPosition;
	private Vector3 _handOffsetRotation;
	private Node3D _lookingAtObject;
	
	public override void _Ready()
	{
		_handOffsetPosition = HandNode.Position;
		_handOffsetRotation = HandNode.Rotation;
	}
    
	public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("grab") && _handTarget != null)
		{
			ReleaseTarget();
		}
		if (@event.IsActionPressed("throw") && _handTarget != null)
		{
			// ThrowTarget(10f);
			if (camera.lookingAtCollisionPosition != null) ThrowTarget((Vector3)camera.lookingAtCollisionPosition);
			else ThrowTarget(20f);
		}

    }
	
	public void GrabTarget(Node3D target)
	{
		if (target is not IGrabbable grabbable)
		{
			GD.PrintErr("Grab target must implement IGrabbable.");
			return;
		}

		_oldTargetParent = target.GetParent<Node3D>();
		_handTarget = target;
	}

	public void GrabCycle(Node3D target)
	{
		if (target is not IGrabbable)
		{
			GD.PrintErr("Grab target must implement IGrabbable.");
			return;
		}

		if (_handTarget == null && target is IGrabbable g)
		{
			_handTarget = target;
			_oldTargetParent = target.GetParent<Node3D>();
			_originalTargetPosition = target.GlobalPosition;
			
			Tween grabTween = CreateTween();
			grabTween.TweenProperty(HandNode, "global_position", _originalTargetPosition, 0.25f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.In);
			
			grabTween.TweenCallback(Callable.From(() => GrabTarget(target)))
				.SetDelay(.05f);
			
			grabTween.TweenCallback(Callable.From(() => target.Reparent(HandNode)));
			grabTween.SetParallel(true);
			grabTween.TweenCallback(Callable.From(() =>
			{
				_handTarget.Position = g.GrabOffset;
			}));
			grabTween.TweenProperty(_handTarget, "rotation", g.GrabOrientation, 0.1f);
			grabTween.SetParallel(false);
			grabTween.TweenMethod(Callable.From<float>(UpdateTargetPosition), 0f, 1f, 0.25f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.In);
		}
		else
		{
			ReleaseTarget();
		}
	}

	private void UpdateTargetPosition(float weight)
	{
		if (_handTarget != null)
		{
			HandNode.GlobalPosition = _originalTargetPosition.Lerp(ToGlobal(_handOffsetPosition), weight);
		}
	}

	public void ReleaseTarget()
	{
		if (_handTarget != null && _handTarget is IGrabbable grabbable)
		{
			grabbable.Release();
			_handTarget.Reparent(_oldTargetParent);
			_handTarget = null;			
		}
	}

	public void ThrowTarget(float force)
	{
		if (_handTarget is IThrowable throwable)
		{
			// throwable.Release();
			_handTarget.Reparent(_oldTargetParent);
			throwable.Throw(-camera.GlobalBasis.Z, force);
			_handTarget = null;
		}
	}

	public void ThrowTarget(Vector3 position)
	{
		if (_handTarget is IThrowable throwable)
		{			
			Vector3 direction = _handTarget.GlobalPosition.DirectionTo(position);
			_handTarget.Reparent(_oldTargetParent);
			throwable.Throw(direction, position);
			_handTarget = null;
		}
	}
}
