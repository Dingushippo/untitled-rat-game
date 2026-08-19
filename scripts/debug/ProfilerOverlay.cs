using Godot;

/// <summary>
/// On-screen readout for <see cref="Profiler"/>, plus a headless benchmark entry point.
///
/// F3 toggles the overlay, F4 clears the samples.
/// Command line: <c>-- --profile</c> prints a report on quit, <c>-- --bench-throw</c> runs the
/// arc simulation benchmark and quits.
/// </summary>
public partial class ProfilerOverlay : CanvasLayer
{
    private const int BENCH_ITERATIONS = 2000;

    /// <summary>Frames to let the scene settle before benchmarking, so bodies exist to cast against.</summary>
    private const int BENCH_SETTLE_FRAMES = 30;

    /// <summary>Discarded up front so JIT compilation doesn't land in the samples as a 40 ms outlier.</summary>
    private const int BENCH_WARMUP_FRAMES = 60;

    /// <summary>Redrawing every frame costs more than the thing being measured.</summary>
    private const float REFRESH_SECONDS = 0.25f;

    private Label _label;
    private float _sinceRefresh;
    private bool _printOnExit;

    private bool _benchActive;
    private int _benchFrame;

    public override void _Ready()
    {
        Layer = 128;
        ProcessMode = ProcessModeEnum.Always;

        _label = new Label
        {
            // Position = new Vector2(815f, 271.5f),
            GrowHorizontal = Control.GrowDirection.Begin,
            Visible = false,
        };
        _label.AddThemeColorOverride("font_color", Colors.White);
        _label.AddThemeFontSizeOverride("font_size", 12);
        _label.AddThemeColorOverride("font_outline_color", Colors.Black);
        _label.AddThemeConstantOverride("outline_size", 8);
        _label.AddThemeConstantOverride("line_spacing", -6);
        AddChild(_label);
        // _label.SetAnchorsPreset(Control.LayoutPreset.CenterRight);
        _label.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.CenterRight);

        _printOnExit = HasFlag("--profile");
        _benchActive = HasFlag("--bench-throw");
    }

    /// <summary>
    /// One simulation per physics frame, exactly as the aim preview runs it. Hammering thousands of
    /// iterations inside a single frame instead would starve the engine and hide the per-frame GC
    /// cost, which is a real part of the price.
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        // Counted here rather than in _Process because the work being measured - raycasts - happens
        // on the physics step, and the two run at different rates.
        Profiler.EndFrame();

        // Deferred: quitting straight from a physics callback tears the tree down mid-frame.
        GetTree().CallDeferred(SceneTree.MethodName.Quit);
    }

    public override void _Process(double delta)
    {
        if (!_label.Visible)
            return;

        _sinceRefresh += (float)delta;
        if (_sinceRefresh < REFRESH_SECONDS)
            return;

        _sinceRefresh = 0f;
        _label.Text = Profiler.Report();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Echo: false } key)
            return;

        if (key.Keycode == Key.F3)
        {
            _label.Visible = !_label.Visible;
            _label.Text = Profiler.Report();
        }
        else if (key.Keycode == Key.F4)
        {
            Profiler.Reset();
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest && _printOnExit)
            GD.Print(Profiler.Report());
    }

    /// <summary>
    /// Drives <c>ThrowType.Simulate</c> directly against the live physics world, which is the only
    /// way to measure it - the raycasts it depends on can't run outside a running scene.
    /// </summary>
    private static bool HasFlag(string flag)
    {
        foreach (string arg in OS.GetCmdlineArgs())
            if (arg == flag)
                return true;

        foreach (string arg in OS.GetCmdlineUserArgs())
            if (arg == flag)
                return true;

        return false;
    }

    private static T FindFirst<T>(Node from)
        where T : class
    {
        if (from is T match)
            return match;

        foreach (Node child in from.GetChildren())
        {
            T found = FindFirst<T>(child);
            if (found is not null)
                return found;
        }

        return null;
    }
}
