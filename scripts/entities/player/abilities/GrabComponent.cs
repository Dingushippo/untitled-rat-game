using Godot;
using Godot.Collections;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

public class GrabComponent
{
    public Rat CurrentGrabbed;
    private Player _player;
    public GrabComponent(Player player)
    {
        _player = player;
    }

    public bool HasGrabbed() => CurrentGrabbed != null;

    public Rat Retrieve()
    {
        Rat rat = CurrentGrabbed;
        CurrentGrabbed = null;
        return rat;
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