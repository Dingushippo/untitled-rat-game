using Godot;
using Godot.Collections;
using System;

public partial class InteractComponent
{
    public float InteractDistance = 2.5f;
    public Dictionary RayResult;
    public InteractAreaComponent ComponentLookedAt;
    private Player _player;

    public InteractComponent(Player player)
    {
        _player = player;
    }

    public void PhysicsUpdate()
    {
        RayResult = null;
        Vector3 rayStart = _player.Camera.GlobalPosition;
        Vector3 rayEnd = rayStart + -_player.Camera.GlobalBasis.Z * InteractDistance;

        // InteractAreaComponent newComponent = null;
        if (Utils.Raycast(_player, rayStart, rayEnd, out Dictionary result, 4))
        {
            RayResult = result;
            InteractAreaComponent newComponent = (InteractAreaComponent)RayResult["collider"];
            if (newComponent != ComponentLookedAt && ComponentLookedAt != null)
            {
                ComponentLookedAt.IsLookedAwayFrom();
            }
            ComponentLookedAt = newComponent;
            ComponentLookedAt.IsLookedAt();
        }
        else if (ComponentLookedAt != null)
        {
            ComponentLookedAt.IsLookedAwayFrom();
            ComponentLookedAt = null;
        }

        if (Input.IsActionJustPressed("interact") && ComponentLookedAt != null)
        {
            ComponentLookedAt.Interact();
        }
    }
}
