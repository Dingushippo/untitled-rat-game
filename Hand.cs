using Godot;
using System;

public partial class Hand : Node3D
{
	
	
	private Node3D _handTarget;
	private Node3D _oldTargetParent;

	private Vector3 _originalPosition;

	public void GrabTarget(Node3D target)
	{
		_oldTargetParent = target.GetParent<Node3D>();
		target.Reparent(this);
		_handTarget = target;
	}

	public void MoveHandToTarget(Node3D target)
	{
		_originalPosition = GlobalPosition; // Store the original position before moving
		Tween moveTween = CreateTween();
		moveTween.TweenProperty(this, "global_transform", target.GlobalTransform, 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);
	}

	public void ReturnHandToOriginalPosition(Vector3 originalPosition)
	{
		// Return to the original position, relative to parent
		Tween moveTween = CreateTween();
		moveTween.TweenProperty(this, "global_transform", new Transform3D(Basis.Identity, originalPosition), 0.5f)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);
	}

	public void GrabCycle(Node3D target)
	{
		if (_handTarget == null)
		{			
			MoveHandToTarget(target);
			GrabTarget(target);
			ReturnHandToOriginalPosition(_originalPosition);
		}
		else
		{			
			ReleaseTarget();
		}
	}

	public void ReleaseTarget()
	{
		if (_handTarget != null)
		{
			_handTarget.Reparent(_oldTargetParent);
			_handTarget = null;
		}
	}
}
