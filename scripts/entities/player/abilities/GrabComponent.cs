using Godot;
using Godot.Collections;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

public class GrabComponent
{
    const float GRAB_DISTANCE = 3f;
    public Rat CurrentGrabbed;
    private Player _player;
    private InteractAreaComponent _interactAreaComponent;
    public GrabComponent(Player player)
    {
        _player = player;
    }

    public void PhysicsUpdate()
    {
        _interactAreaComponent = _player.InteractComponent.ComponentLookedAt;
    }

    public bool HasGrabbed() => CurrentGrabbed != null;

    public bool CanGrab(out Rat rat)
    {
        rat = null;
        if (_interactAreaComponent != null && _interactAreaComponent.GetOwner() is Rat ratOut)
        {
            rat = ratOut;
            return true;
        }
        return false;
    }

    public Rat Retrieve()
    {
        Rat rat = CurrentGrabbed;
        CurrentGrabbed = null;
        return rat;
    }

    public bool TryGrab()
    {
        if (CanGrab(out Rat rat))
        {
            InjectGrabState(rat);
            return true;
        }
        return false;
    }

    public void InjectGrabState(Rat rat)
    {
        CurrentGrabbed = rat;
        RatGrabState grabState = new RatGrabState(rat, _player);
        rat.InjectState("grab", grabState);
    }

    public void Release()
    {
        CurrentGrabbed.SetIdle();
        CurrentGrabbed = null;
    }
}