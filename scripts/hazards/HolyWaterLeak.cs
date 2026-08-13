using Godot;
using System;
using System.Linq;

public partial class HolyWaterLeak : Node3D, ICatchArea, IRitualInteract
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
        EventBus.Publish(Event.SetDisruptFacilityInRange, GlobalPosition, DisruptionRadius, disrupting);
    }

    public bool TryGetThrowTarget(Vector3 from, Rat rat, out ThrowTarget target)
    {
        target = default;

        if (Slot.IsOccupied) return false;

        target = ThrowTarget.Slot(Slot);
        return true;
    }

    public bool TryGetRitualPosition(RitualBase ritual, out Vector3 position)
    {
        position = default;
        if (!IsRitualValidFor(ritual.RitualResource.Id))
            return false;
        position = GlobalPosition;
        return true;
    }

    public bool IsRitualValidFor(string ritualId)
    {
        string[] validRituals = ["sealing_ritual"];
        return validRituals.Contains(ritualId);
    }

    public void OnRitualComplete(RitualBase ritual)
    {
        Tween removeTween = CreateTween();
        if (Particles.Emitting)
        {
            SetDisruptStatus(false);
            removeTween.TweenInterval(Particles.Lifetime);
        }
        removeTween.TweenCallback(Callable.From(() => PopOutRat()));
        removeTween.TweenCallback(Callable.From(() => QueueFree()));
    }

    private void PopOutRat()
    {
        if (!Slot.IsOccupied) return;

        Rat rat = Slot.Occupant;
        rat.ChangeState<RatIdleState>();
        rat.Velocity += Vector3.Up * 10f;
    }
}
