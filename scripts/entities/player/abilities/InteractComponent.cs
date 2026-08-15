using Godot;
using Godot.Collections;

public partial class InteractComponent
{
    public float InteractDistance = 2.5f;
    public float InteractHoldTime = 0.5f;
    public Dictionary RayResult;
    public IInteract ComponentLookedAt;
    private Player _player;
    private float _timer;

    /// <summary>
    /// True when the thing under the crosshair actually handles interaction. "interact" doubles as
    /// grab/drop, so the hand states use this to know when to yield the key.
    /// </summary>
    public bool IsLookingAtHandler => ComponentLookedAt is not null && ComponentLookedAt.HasHandler;
    public InteractComponent(Player player)
    {
        _player = player;
    }

    public void PhysicsUpdate(float delta)
    {
        RayResult = null;
        Vector3 rayStart = _player.Camera.GlobalPosition;
        Vector3 rayEnd = rayStart + -_player.Camera.GlobalBasis.Z * InteractDistance;

        GodotObject newComponent = null;
        if (RaycastUtils.Ray(
            _player,
            rayStart,
            rayEnd, out Dictionary result,
            PhysicsLayers.GetOrMask(PhysicsLayers.WORLD, PhysicsLayers.INTERACT, PhysicsLayers.FACILITY),
            accept: o => o is IInteract i && i.IsAvailableTo(_player)
        ))
        {
            RayResult = result;
            newComponent = result["collider"].As<GodotObject>();
        }

        IInteract interact = newComponent as IInteract;

        if (interact != ComponentLookedAt)
        {
            ComponentLookedAt?.IsLookedAwayFrom();
            ComponentLookedAt = interact;
        }
        if (interact is not null)
        {
            ComponentLookedAt?.IsLookedAt();
            if (Input.IsActionPressed("interact"))
            {
                _timer += delta;
            }
            bool isHeld = _timer > InteractHoldTime;
            if ((Input.IsActionJustReleased("interact") || isHeld) && ComponentLookedAt != null)
            {
                ComponentLookedAt.Interact(_player, isHeld);
                _timer = 0;
            }
        }
    }
}