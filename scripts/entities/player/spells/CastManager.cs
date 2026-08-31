using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CastManager : Node
{
    [Export] public SpellData CurrentSpellResource;
    [Export] public SpellData[] SpellResources;
    [Export] public Node3D CastNode;

    // private List<Spell> _activeSpells;
    // private Spell _currentSpell;

    private float _chargeTimer;
    private bool _isCharging;

    public override void _PhysicsProcess(double delta)
    {

    }

    public void Cast(SpellData spell, Vector3 targetPosition, Array<Node3D> targetNode = null) // on release
    {
        if (spell.SpellScene == null)
            return;


        SpellPayload payload = new(CastNode, targetPosition);

        if (spell.SpellScene.Instantiate() is SpellBase spellInstance)
        {
            GD.Print("test");
            GetTree().CurrentScene.AddChild(spellInstance);
            spellInstance.Initialize(CastNode, payload);
        }
    }
}