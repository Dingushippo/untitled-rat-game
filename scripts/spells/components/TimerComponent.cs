using Godot;


[GlobalClass]
public partial class TimerComponent : TimerSpellComponent
{
    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);
        OneShot = true;
        Timeout += TimerExit;
        Start();
    }

    private void TimerExit() => RaiseComplete(_payload);
}