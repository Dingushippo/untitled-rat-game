using Godot;

public partial class PlayerSlideState : PlayerState
{
    [Export] private float _slideDecay = 0.98f;
    [Export] private float _maxSlideSpeed = 20f;
    [Export] private float _slideVelocityBoost = 2f;
    [Export] private float _slideExitVelocity = 1f;

    private float _currentSlideSpeed;
}
