using System.Linq;
using Godot;
using Godot.Collections;

public partial class GameManager : Node
{
    [Export]
    public RunTuning Tuning;

    [Export]
    public Array<TimelineResource> Timeline;
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    private HierarchicalStateMachine<GameState> _fsm;

    public bool HasFatalDataError { get; private set; } = false;

    public override void _Process(double delta)
    {
        DebugDraw.Clear(); // Feels like a good place to have it
        _fsm.StateProcess((float)delta);
    }

    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);

    public override void _UnhandledInput(InputEvent @event) => _fsm.StateUnhandledInput(@event);

    public override void _Input(InputEvent @event) => _fsm.StateInput(@event);

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this))
            return;

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

        _fsm = new(this);

        _fsm.Add(new GameMenuState(this));
        _fsm.Add(new GameRunState(this));
        _fsm.InitState<GameMenuState>();
        _fsm.Debug = Tuning.DebugStateTransitions;
    }

    public void SetDataErrorFlag() => HasFatalDataError = true;
}
