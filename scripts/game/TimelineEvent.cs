using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TimelineEvent : Resource
{
    [Export] public string TimeStamp; // e.g. 10:30
    [Export] public TimelineEventType Type;
    [Export] public Dictionary<string, string> Data;
}

public enum TimelineEventType
{
    Hazard
}