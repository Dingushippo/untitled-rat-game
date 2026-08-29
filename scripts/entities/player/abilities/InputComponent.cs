using Godot;

[GlobalClass]
public partial class InputComponent : Node
{
    public Vector2 DirectionRaw { get; private set; }
    public Vector3 Direction { get; private set; }

    public bool LeftArmAction { get; private set; }
    public bool RightArmAction { get; private set; }

    public bool WantsJump { get; private set; }
    public bool WantsSprint { get; private set; }
    public bool WantsSlide { get; private set; }
    public bool WantsDash { get; private set; }
    public bool WantsCrouch { get; private set; }

    public bool NoMovement => DirectionRaw == Vector2.Zero;

    [Export] public float JumpBufferTime = 0.15f; // Standard 150ms buffer

    private Player _player;
    private float _jumpBufferTimer = float.MaxValue;

    public void Init(Player player)
    {
        _player = player;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        // 1. Raw Input Vector
        DirectionRaw = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        // 2. Camera-Relative Movement Vector (Forward = -Basis.Z)
        if (_player?.Camera != null)
        {
            Transform3D camTransform = _player.Camera.GlobalTransform;
            Vector3 forward = -camTransform.Basis.Z;
            Vector3 right = camTransform.Basis.X;

            forward.Y = 0;
            right.Y = 0;

            Direction = (forward.Normalized() * -DirectionRaw.Y + right.Normalized() * DirectionRaw.X).Normalized();
        }

        // 3. Action States
        LeftArmAction = Input.IsActionPressed("left_hand");
        RightArmAction = Input.IsActionPressed("right_hand");
        WantsSprint = Input.IsActionPressed("sprint");
        WantsDash = Input.IsActionJustPressed("dash");
        WantsCrouch = Input.IsActionPressed("crouch");

        // 4. Jump Buffer Logic
        if (Input.IsActionJustPressed("jump"))
        {
            _jumpBufferTimer = 0f;
        }
        else
        {
            _jumpBufferTimer += dt;
        }

        WantsJump = _jumpBufferTimer <= JumpBufferTime;
    }
    public void ConsumeJump()
    {
        _jumpBufferTimer = float.MaxValue;
        WantsJump = false;
    }
}