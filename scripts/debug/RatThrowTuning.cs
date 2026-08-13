using Godot;

public partial class RatThrowTuning : Node3D
{
    [Export] public Player Player;
    [Export] public Rat rat;
    [Export] public ThrowTuning throwTuning;
    [Export] public ThrowPreviewTuning throwPreviewTuning;
    [Export] public RatFlightTuning ratFlightTuning;
    [Export] public PlayerCameraTuning playerCameraTuning;

    private float _ratFlyCounter = 0;
    private Vector3 _startingPosition;
    private bool _counterEnabled = false;


    public override void _Ready()
    {
        EventBus.Subscribe<RatThrown>(OnRatThrown);
        EventBus.Subscribe<RatLanded>(OnRatLanded);

        GD.Print("Ready");
    }

    public override void _Process(double delta)
    {
        if (!_counterEnabled) return;

        _ratFlyCounter += (float)delta;
    }

    private void OnRatThrown(RatThrown _)
    {
        GD.Print("Throw");
        _startingPosition = Player.GlobalPosition;
        _counterEnabled = true;
    }

    private void OnRatLanded(RatLanded _)
    {
        _counterEnabled = false;
        float distance = _startingPosition.DistanceTo(rat.GlobalPosition);
        GD.Print($"Traveled {distance}m in {_ratFlyCounter} seconds");
        _ratFlyCounter = 0;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Key1)
        {
            Player.GrabComponent.InjectGrabState(rat);
        }
    }
}