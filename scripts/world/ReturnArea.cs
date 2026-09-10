using Godot;

public partial class ReturnArea : Area3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += ReturnToHome;
    }

    private void ReturnToHome(Node3D obj)
    {
        if (obj is SpellBase spell)
            spell.QueueFree();

        if (obj is Player player)
            player.GlobalPosition = Vector3.One * 2;
    }

    public override void _ExitTree() => BodyEntered -= ReturnToHome;
}
