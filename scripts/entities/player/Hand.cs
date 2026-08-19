using Godot;

public partial class Hand : Node3D
{
    // Called when the node enters the scene tree for the first time.
    [Export]
    public Player Player;
    public Rat Held;
    private FiniteStateMachine<HandState> _fsm;

    public override void _Ready()
    {
        _fsm = new(this);
        _fsm.Add(new HandEmptyState(this));
        _fsm.Add(new HandHoldingState(this));
        _fsm.InitState<HandEmptyState>();
        _fsm.Debug = false;
    }

    public void Release()
    {
        Held = null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) => _fsm.StateProcess((float)delta);

    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);

    public override void _Input(InputEvent @event) => _fsm.StateInput(@event);

    public override void _UnhandledInput(InputEvent @event) => _fsm.StateUnhandledInput(@event);
}
