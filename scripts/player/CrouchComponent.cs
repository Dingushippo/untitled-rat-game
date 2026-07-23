using Godot;
using System.IO;

public partial class CrouchComponent
{
    // [Export] public Player _player;
    const float STANDING_HEIGHT = 1.5f;
    const float CROUCHING_HEIGHT = 1f;
    const float CROUCH_ANIM_DURATION = 0.15f;

    public bool IsCrouching { get; private set; }
    public bool ToggleCrouch = true;

    private bool _crouchToggled = false;
    private readonly Player _player;


    public CrouchComponent(Player player)
    {
        _player = player;
    }

    public void Update()
    {
        if (ToggleCrouch)
            UpdateToggleMode();
        else
            UpdateHoldMode();
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

    private void Crouch()
    {
        if (IsCrouching)
            return;

        IsCrouching = true;
        TweenCrouchPos(CROUCHING_HEIGHT);
    }

    private void TryStand()
    {
        if (!IsCrouching)
            return;

        if (CeilingBlocked())
            return;

        IsCrouching = false;
        TweenCrouchPos(STANDING_HEIGHT);
    }

    private bool CeilingBlocked()
    {
        // ShapeCast3D or PhysicsShapeQuery
        return false;
    }

    private void TweenCrouchPos(float height)
    {
        Tween crouchTween = _player.CreateTween();
        crouchTween.TweenProperty(_player.Camera, "position:y", height, CROUCH_ANIM_DURATION);
    }
}