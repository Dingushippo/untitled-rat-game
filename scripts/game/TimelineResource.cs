using Godot;
using Godot.Collections;


[GlobalClass]
public partial class TimelineResource : Resource
{
    [Export(PropertyHint.Range, "1,3,1")] public int Day;
    [Export] public Array<TimelineEvent> Events { get; set; } = new();
}