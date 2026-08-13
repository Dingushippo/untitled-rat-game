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
        rat.GetState<RatGrabState>().Configure(_player);
        rat.ChangeState<RatGrabState>();
    }

    public void Release()
    {
        CurrentGrabbed.ChangeState<RatIdleState>();
        CurrentGrabbed = null;
    }
}