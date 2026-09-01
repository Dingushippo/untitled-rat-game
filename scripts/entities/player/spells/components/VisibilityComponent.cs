using Godot;

[GlobalClass]
public partial class VisibilityComponent : SpellComponent
{
    [Export] public Node3D[] Hide;
    [Export] public Node3D[] Show;

    public override void Initialize(Node3D spell, SpellPayload payload)
    {
        base.Initialize(spell, payload);

        foreach (Node3D node in Hide)
            node.Hide();
        foreach (Node3D node in Show)
            node.Show();
    }
}