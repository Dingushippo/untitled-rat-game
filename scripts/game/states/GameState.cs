using Godot;

public partial class GameState : State
{
    private protected GameManager _manager;

    public override void Init(Node owner, HierarchicalStateMachine stateMachine, State parent = null)
    {
        base.Init(owner, stateMachine, parent);
        if (owner is not GameManager manager)
        {
            GD.PushError($"{this} owner is not the game manager");
            return;
        }
        _manager = manager;
    }
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
