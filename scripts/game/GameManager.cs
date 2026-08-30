using System.Linq;
using Godot;
using Godot.Collections;

public partial class GameManager : Node
{
    [Export]
    public RunTuning Tuning;

    [Export]
    public Array<TimelineResource> Timeline;

    [Export]
    private HierarchicalStateMachine _hfsm;

    private static GameManager _instance;
    public static GameManager Instance => _instance;


    public bool HasFatalDataError { get; private set; } = false;

    public override void _Process(double delta)
    {

    }

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this))
            return;

        _hfsm.Init(this);

        // Check if current scene is a tool script, queue free and return in that casen
        string[] excludes = ["scenes/tools/", "scenes/debug/"];
        string currentScenePath = GetTree().CurrentScene.SceneFilePath;
        GD.Print($"Current scene: {currentScenePath}");
        if (excludes.Any(x => currentScenePath.Contains(x)))
        {
            // Just disabling to prevent error messages
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            QueueFree();
            return;
        }
    }

    public void SetDataErrorFlag() => HasFatalDataError = true;
}
