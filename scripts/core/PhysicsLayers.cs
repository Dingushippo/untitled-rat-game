public static class PhysicsLayers
{
    public static uint WORLD = 1;
    public static uint PLAYER = 2;
    public static uint PLAYER_HITBOX = 4;
    public static uint PLAYER_HURTBOX = 8;
    public static uint ENTITY = 16;
    public static uint ENTITY_HITBOX = 32;
    public static uint ENTITY_HURTBOX = 64;
    public static uint INTERACT = 128;

    public static uint GetOrMask(params uint[] layers)
    {
        if (layers.Length == 0) return 0;
        uint mask = 0;
        foreach (int layer in layers)
        {
            mask |= (uint)layer;
        }
        return mask;
    }
}