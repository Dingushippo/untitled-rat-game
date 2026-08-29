using Godot;
using System.Runtime.CompilerServices;

[GlobalClass]
public partial class InputComponent : Node
{
    public Vector2 DirectionRaw;
    public Vector3 Direction;
    public bool LeftArmAction;
    public bool RightArmAction;
    public bool WantsJump;
    public bool WantsSprint;
    public bool WantsDash;
    public bool WantsCrouch;

    private Player _player;

    public bool NoMovement => DirectionRaw == Vector2.Zero;
    public void Init(Player player)
    {
        _player = player;
    }

    public override void _Process(double delta)
    {
        DirectionRaw = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Direction = new Vector3(DirectionRaw.X, 0, DirectionRaw.Y).Rotated(Vector3.Up, _player.Camera.GlobalRotation.Y);

        LeftArmAction = Input.IsActionPressed("left_hand");
        RightArmAction = Input.IsActionPressed("right_hand");

        WantsJump = Input.IsActionPressed("jump");
        WantsSprint = Input.IsActionPressed("sprint");
        WantsDash = Input.IsActionPressed("dash");
        WantsCrouch = Input.IsActionPressed("crouch");
    }
}