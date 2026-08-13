using Godot;
using System;

public partial class ReturnArea : Area3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        BodyEntered += ReturnToHome;
    }

    private void ReturnToHome(Node3D obj)
    {
        if (obj is Rat rat)
        {
            rat.ChangeState<RatFallingState>();
            rat.GlobalPosition = rat.HomePosition + Vector3.Up * 0.5f;
        }

        if (obj is Player player)
        {
            player.GlobalPosition = Vector3.One * 2;
        }
    }
}
