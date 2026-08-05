using Godot;

[GlobalClass, Tool]
public partial class RunTuning : Resource
{
    [Export] public float DayLength = 420f;
    [Export] public int[] Quotas = { 4, 8, 14 };
    [Export] public int[] RatsSpawnedPerDay = { 6, 2, 2 };
    [Export] public bool FixedSeed = false;
    [Export] public ulong Seed = 1;
    [Export] public bool DebugStateTransitions = false;
    [Export] public bool DebugKeys = false;
}