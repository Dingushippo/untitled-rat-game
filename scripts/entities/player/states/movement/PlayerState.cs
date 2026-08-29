

using Godot;

public partial class PlayerState : TypedState<PlayerState>
{
    private protected Player _player;

    protected Vector3 _velocity => _player.Velocity;
    protected void SetVelocity(Vector3 velocity)
    {
        _player.Velocity = velocity;
    }

    protected void MoveAndSlide() => _player.MoveAndSlide();

    public override void Init(Node owner, HierarchicalStateMachine<PlayerState> stateMachine, State parent)
    {
        base.Init(owner, stateMachine);
        _player = (Player)owner;
    }
}

/*
using Godot;

public partial class PlayerNewState : PlayerState
{
    public PlayerNewState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void IntegrateForces(PhysicsDirectBodyState3D state) { }
    public override void Enter(State previous = null) { }
    public override void HandleInput(InputEvent @event) { }
    public override void HandleUnhandledInput(InputEvent @event) { }
    public override void Exit() { }
}
*/
