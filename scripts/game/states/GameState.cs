
public class GameState : State
{
    private protected GameManager _manager;

    public GameState(GameManager owner) { _manager = owner; }
}

/* Template

using Godot;


public class GameNewState : RatState
{
    public GameNewState(Rat owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}

*/