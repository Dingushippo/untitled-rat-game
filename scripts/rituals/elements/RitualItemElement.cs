using Godot;


[GlobalClass, Tool]
public partial class RitualItemElement : RitualElement
{
    [Export] public ItemDef Item;
    [Export] public int Amount;
}