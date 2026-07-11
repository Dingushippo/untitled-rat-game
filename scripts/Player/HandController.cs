using Godot;
using System;


[GlobalClass]
public partial class HandController : Node3D
{
	[Export] public Node3D HandNode;

	private Node3D _handTarget;
	private Node3D _oldTargetParent;
	private Vector3 _originalTargetPosition;
	private Vector3 _handOffsetPosition;
	private Vector3 _handOffsetRotation;
	


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
			ThrowTarget(_handTarget, 10f);
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

		if (_handTarget == null)
		{
			_handTarget = target;
			_oldTargetParent = target.GetParent<Node3D>();
			_originalTargetPosition = target.GlobalPosition;

			// _newTargetTransform.Scale = Vector3.One;
			
			Tween grabTween = CreateTween();
			grabTween.TweenProperty(HandNode, "global_position", _originalTargetPosition, 0.25f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.In);
			grabTween.TweenCallback(Callable.From(() => GrabTarget(target)))
				.SetDelay(.05f);
			grabTween.TweenCallback(Callable.From(() => target.Reparent(HandNode)));
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

	public void ThrowTarget(Node3D target, float force)
	{
		if (target is IThrowable throwable)
		{
			// throwable.Release();
			_handTarget.Reparent(_oldTargetParent);
			throwable.Throw(-GetParent<Camera3D>().GlobalBasis.Z, force);
			_handTarget = null;
		}
	}

    // public override void _Process(double delta)
    // {
    //     GD.Print($"Global rotation: {GlobalBasis.Z}");
    // }

}
