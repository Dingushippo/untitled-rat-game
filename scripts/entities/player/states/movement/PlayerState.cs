

using Godot;

public partial class PlayerState : State
{
    private protected Player _player;

    protected Vector3 _velocity => _player.Velocity;
    protected void SetVelocity(Vector3 velocity)
    {
        _player.Velocity = velocity;
    }

    protected void AddVelocity(Vector3 velocity)
    {
        _player.Velocity += velocity;
    }

    protected void MoveAndSlide() => _player.MoveAndSlide();

    public override void Init(Node owner, HierarchicalStateMachine stateMachine, State parent)
    {
        base.Init(owner, stateMachine, parent);
        if (owner is not Player player)
        {
            GD.PushError($"owner is not player"); return;
        }
        _player = player;
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
