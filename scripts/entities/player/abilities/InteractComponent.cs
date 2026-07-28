using Godot;
using Godot.Collections;
using System;

public partial class InteractComponent
{
    public float InteractDistance = 2.5f;
    public Dictionary RayResult;
    public InteractAreaComponent ComponentLookedAt;
    private Player _player;

    /// <summary>
    /// True when the thing under the crosshair actually handles interaction. "interact" doubles as
    /// grab/drop, so the hand states use this to know when to yield the key.
    /// </summary>
    public bool IsLookingAtHandler => ComponentLookedAt is not null && ComponentLookedAt.HasHandler;

    public InteractComponent(Player player)
    {
        _player = player;
    }

    public void PhysicsUpdate()
    {
        RayResult = null;
        Vector3 rayStart = _player.Camera.GlobalPosition;
        Vector3 rayEnd = rayStart + -_player.Camera.GlobalBasis.Z * InteractDistance;

        InteractAreaComponent newComponent = null;
        if (Utils.Raycast(_player, rayStart, rayEnd, out Dictionary result, InteractAreaComponent.INTERACT_LAYER))
        {
            RayResult = result;
            newComponent = result["collider"].As<GodotObject>() as InteractAreaComponent;
        }

        if (newComponent != ComponentLookedAt)
        {
            ComponentLookedAt?.IsLookedAwayFrom();
            ComponentLookedAt = newComponent;
        }
        ComponentLookedAt?.IsLookedAt();

        if (Input.IsActionJustPressed("interact") && ComponentLookedAt != null)
        {
            ComponentLookedAt.Interact(_player);
        }
    }
}
