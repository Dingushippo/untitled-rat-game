using Godot;

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

    public void Init(Player player)
    {
        _player = player;
    }

    public override void _Process(double delta)
    {
        DirectionRaw = Input.GetVector("left", "right", "forward", "backward");
        Direction = _player.Camera.GlobalBasis * new Vector3(DirectionRaw.X, 0, DirectionRaw.Y);

        LeftArmAction = Input.IsActionPressed("left_arm");
        RightArmAction = Input.IsActionPressed("right_arm");

        WantsJump = Input.IsActionPressed("jump");
        WantsSprint = Input.IsActionPressed("sprint");
        WantsDash = Input.IsActionPressed("dash");
        WantsCrouch = Input.IsActionPressed("crouch");
    }
}