using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class RitualCircleResource : Resource
{
    [Export] public Array<RitualElement> Elements = [];
}