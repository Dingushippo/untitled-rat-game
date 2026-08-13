
public class GameState : State<GameState>
{
    private protected GameManager _manager;

    public GameState(GameManager owner) { _manager = owner; }
}

/* Template

using Godot;


public class GameNewState : GameState
{
    public GameNewState(GameManager owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}

*/