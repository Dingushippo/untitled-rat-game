using Godot;

public interface IRitualInteract
{
    public bool TryGetRitualPosition(RitualBase ritual, out Vector3 position);
    public bool IsRitualValidFor(string ritualId);
    public void OnRitualComplete(RitualBase ritual);
}