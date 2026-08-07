using Godot;
using Godot.Collections;


[GlobalClass, Tool]
public partial class RitualResource : Resource
{
    [Export] public Array<RitualCircleResource> ritualCircles;
}