using Godot;

public partial class GameManager : Node
{
    private FiniteStateMachine _fsm;

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);
    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);
    public override void _UnhandledInput(InputEvent @event) => _fsm.StateUnhandledInput(@event);
    public override void _Input(InputEvent @event) => _fsm.StateInput(@event);

    public override void _Ready()
    {
        _fsm = new(this);
        _fsm.Add("menu", new GameMenuState(this));
        _fsm.Add("run", new GameRunState(this));
        _fsm.Add("result", new GameResultState(this));
        _fsm.InitState("menu");
        _fsm.Debug = true;
    }
}