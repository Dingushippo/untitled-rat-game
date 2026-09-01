using Godot;

[GlobalClass]
public partial class TrailComponent : TrailSpellComponent
{
    [Export] public Node StopOnComponentCompleted;

    public SpellPayload Payload { get; set; }

    public override void _Ready()
    {
        Emitting = false;
    }

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        Emitting = true;

        if (StopOnComponentCompleted is not ISpellComponent component)
        {
            GD.PushError($"Node {StopOnComponentCompleted.Name} does not implement ISpellComponent");
            return;
        }

        component.OnComplete += StopParticles;

        GD.Print($"Invoking oncomplete particles, payload {payload.TargetPosition}");
        RaiseComplete(_payload);
    }

    private void StopParticles(SpellPayload _)
    {
        Emitting = false;
        Reparent(GetTree().CurrentScene, true);
        // await ToSignal(GetTree().CreateTimer(Lifetime), Timer.SignalName.Timeout);
        // QueueFree();
    }
}