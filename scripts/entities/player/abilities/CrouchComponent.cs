using Godot;

public partial class CrouchComponent
{
    // [Export] public Player _player;
    public const float CROUCH_OFFSET = -0.6f;
    public const float CROUCH_ANIM_DURATION = 0.15f;

    public bool IsCrouching { get; private set; }
    public bool Enabled = true;
    public bool ToggleCrouch = true;
    private bool _crouchToggled = false;
    private bool _canStand = true;
    private float _colliderHeight;
    private float _colliderYPos;
    private readonly Player _player;

    private Tween _crouchTween;


    public CrouchComponent(Player player)
    {
        _player = player;
        _colliderHeight = (_player.Collider.Shape as CapsuleShape3D).Height;
        _colliderYPos = _player.Collider.Position.Y;
    }

    public void Update()
    {
        if (!Enabled) return;
        if (ToggleCrouch)
            UpdateToggleMode();
        else
            UpdateHoldMode();
        CheckCeilingBlocked();
    }

    private void UpdateToggleMode()
    {
        if (Input.IsActionJustPressed("crouch"))
            _crouchToggled = !_crouchToggled;

        if (_crouchToggled)
            Crouch();
        else
            TryStand();
    }

    private void UpdateHoldMode()
    {
        if (Input.IsActionPressed("crouch"))
            Crouch();
        else
            TryStand();
    }

    public void Crouch()
    {
        if (IsCrouching)
            return;

        IsCrouching = true;
        TweenCrouchOffset(CROUCH_OFFSET);
    }

    public void TryStand()
    {
        if (!IsCrouching)
            return;

        if (!_canStand)
            return;

        _crouchToggled = false;
        IsCrouching = false;
        TweenCrouchOffset(0f);
    }

    private void CheckCeilingBlocked()
    {
        if (!IsCrouching) return;

        if (RaycastUtils.Ray(_player, _player.GlobalPosition, Vector3.Up * CROUCH_OFFSET, out _))
        {
            _canStand = false;
        }
        _canStand = true;

    }

    private void TweenCrouchOffset(float height)
    {
        if (_crouchTween is not null)
        {
            _crouchTween.Kill();
        }
        _crouchTween = _player.CreateTween();
        _crouchTween.SetParallel(true);
        _crouchTween.TweenProperty(_player.Camera, "YOffset", height, CROUCH_ANIM_DURATION);
        _crouchTween.TweenProperty(_player.Collider.Shape as CapsuleShape3D, "height", _colliderHeight + height, CROUCH_ANIM_DURATION);
        _crouchTween.TweenProperty(_player.Collider, "position:y", _colliderYPos + height / 2, CROUCH_ANIM_DURATION);
    }
}