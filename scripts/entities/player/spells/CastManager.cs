using Godot;

[GlobalClass]
public partial class CastManager : Node
{
    [Export] public SpellData CurrentSpellResource;
    [Export] public SpellData[] SpellResources;
    [Export] public Node3D CastNode;

    public void Cast(SpellData spell) // on release
    {
        if (spell.SpellScene == null)
            return;

        SpellPayload payload = new(CastNode);

        if (spell.SpellScene.Instantiate() is SpellBase spellInstance)
        {
            GetTree().CurrentScene.AddChild(spellInstance);
            spellInstance.Initialize(CastNode, payload);
        }
    }
}