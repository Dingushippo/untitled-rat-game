using Godot;
using System;

public partial class HolyWaterLeak : Node3D, ICatchArea
{
    [Export] public WorkSlot Slot;
    [Export] public GpuParticles3D Particles;
    [Export] public float DisruptionRadius = 10f;

    public float ColliderTopY { get => 0; set => throw new NotImplementedException(); }

    public override void _Ready()
    {
        Slot.Entered += () => SetDisruptStatus(false);
        Slot.Exited += () => SetDisruptStatus(true);
        SetDisruptStatus(true);
    }

    private void SetDisruptStatus(bool disrupting)
    {
        Particles.Emitting = disrupting;
        EventBus.Publish(Event.SetDisruptProductionInRange, GlobalPosition, DisruptionRadius, disrupting);
    }


    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = default;

        if (Slot.IsOccupied) return false;

        target = ThrowTarget.Slot(Slot);
        return true;
    }
}
