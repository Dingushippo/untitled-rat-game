using Godot;

public static class Singleton
{
    public static bool ClaimOrFree<T>(ref T slot, T candidate) where T : Node
    {
        if (slot is not null && GodotObject.IsInstanceValid(slot))
        {
            candidate.QueueFree();
            return false;
        }
        slot = candidate;
        return true;
    }
}

