using Godot;

public partial class GameManager : Node
{
    [Export] public RunTuning Tuning;
    [Export] public bool Disabled;
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    private FiniteStateMachine _fsm;

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);
    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);
    public override void _UnhandledInput(InputEvent @event) => _fsm.StateUnhandledInput(@event);
    public override void _Input(InputEvent @event) => _fsm.StateInput(@event);

    public override void _Ready()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this)) return;

        AssertTuning();
        _fsm = new(this);

        if (Disabled)
        {
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            return;
        }

        _fsm.Add("menu", new GameMenuState(this));
        _fsm.Add("run", new GameRunState(this));
        _fsm.Add("result", new GameResultState(this));
        _fsm.InitState("menu");
        _fsm.Debug = Tuning.DebugStateTransitions;
    }

    private void AssertTuning()
    {
        if (Tuning == null)
        {
            GD.PushError("GameManager Tuning is null, please assign a RunTuning resource in the inspector.");
            return;
        }
        if (Tuning.RatsSpawnedPerDay.Length != Tuning.Quotas.Length)
        {
            GD.PushError("RatsSpawnedPerDay and Quotas arrays must be the same length.");
            return;
        }
    }
}