using Godot;
using System;

public partial class InteractRaycast : RayCast3D
{
	InteractComponent componentLookedAt;
    public override void _PhysicsProcess(double delta)
    {
        // Call LookedAwayFrom on previous component if we look away, or on a different one
		if (componentLookedAt != null && !IsColliding())
		{
			componentLookedAt.IsLookedAwayFrom();
			componentLookedAt = null;
		}
		if (!IsColliding()) return;


		InteractComponent newComponent = GetCollider() as InteractComponent;
		if (newComponent != componentLookedAt)
		{
			componentLookedAt?.IsLookedAwayFrom();
			componentLookedAt = newComponent;
			componentLookedAt.IsLookedAt();
		}		
		if (Input.IsActionJustPressed("interact"))
		{
			componentLookedAt.Interact();
		}
		// 
    }
}
