using Godot;
using Godot.Collections;
using System.Diagnostics.Tracing;

public class GrabComponent
{
    const float GRAB_DISTANCE = 3f;
    public Rat CurrentGrabbed;
    private Player _player;
    private Dictionary _rayResult;
    public GrabComponent(Player player)
    {
        _player = player;
    }

    public void PhysicsUpdate()
    {
        _rayResult = null;
        Vector3 rayStart = _player.Camera.GlobalPosition;
        Vector3 rayEnd = rayStart + -_player.Camera.GlobalBasis.Z * GRAB_DISTANCE;
        if (Utils.Raycast(_player, rayStart, rayEnd, out Dictionary result, 4))
        {
            _rayResult = result;
        }
    }

    public bool HasGrabbed() => CurrentGrabbed != null;

    public bool CanGrab() => _rayResult != null;

    public Rat Retrieve()
    {
        Rat rat = CurrentGrabbed;
        CurrentGrabbed = null;
        return rat;
    }

    public bool TryGrab()
    {
        if (!CanGrab()) return false;
        if ((GodotObject)_rayResult["collider"] is InteractComponent component)
        {
            CurrentGrabbed = (Rat)component.GetParent();
            RatGrabState grabState = new RatGrabState(CurrentGrabbed, _player);
            CurrentGrabbed.InjectState("grab", grabState);
        }
        return true;
    }

    public void Release()
    {
        CurrentGrabbed.SetIdle();
        CurrentGrabbed = null;
    }
}