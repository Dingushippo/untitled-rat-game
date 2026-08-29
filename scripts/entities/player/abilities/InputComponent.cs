using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class InputComponent : Node
{
    public Vector2 DirectionRaw;
    public Vector3 Direction;
    public bool LeftArmAction;
    public bool RightArmAction;
    public bool WantsJump;
    public bool WantsSprint;
    public bool WantsSlide;
    public bool WantsDash;
    public bool WantsCrouch;
    private Player _player;
    public bool NoMovement => DirectionRaw == Vector2.Zero;

    private Dictionary<string, float> _actionBuffers = new();

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

        WantsJump = BufferedAction("jump", 0.5f, (float)delta);
        WantsSprint = Input.IsActionPressed("sprint");
        WantsDash = Input.IsActionPressed("dash");
        WantsCrouch = Input.IsActionPressed("crouch");
    }

    private bool BufferedAction(string action, float bufferLength, float delta)
    {
        if (!_actionBuffers.ContainsKey(action))
        {
            _actionBuffers.Add(action, float.MaxValue);
        }
        if (Input.IsActionPressed(action))
        {
            _actionBuffers[action] = 0f;
            return true;
        }

        _actionBuffers[action] += delta;

        return _actionBuffers[action] < bufferLength;
    }
}