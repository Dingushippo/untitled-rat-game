using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class FacilityDef : Resource
{
    [Export] public string Id;                 // "crypt_garden"
    [Export] public string DisplayName;
    [Export] public float CatchRadius = 2f;
}