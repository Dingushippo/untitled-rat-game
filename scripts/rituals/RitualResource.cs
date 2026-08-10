using Godot;
using Godot.Collections;


[GlobalClass, Tool]
public partial class RitualResource : Resource
{
    [Export] public string Id = "new_id";
    [Export] public string DisplayName = "new_name";
    [Export] public string Description = "new_description";
    [Export] public float RitualTime = 10f;
    [Export] public bool RequiresInteract = false;
    [Export] public Array<RitualCircleResource> RitualCircles = [];
}