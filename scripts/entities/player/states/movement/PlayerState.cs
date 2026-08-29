using Godot;

public class PlayerState : TypedState<PlayerState>
{
    private protected Player _player;

    public PlayerState(Player owner)
    {
        _player = owner;
    }

    // Setting the base movement integration here, because most movement states reuse the same thing
    public override void IntegrateForces(PhysicsDirectBodyState3D state)
    {
        _player.CheckOnFloor(state);
        _player.CheckOnWall(state);
        _player.HandleDirectionalMovement(state);
        _player.HandleImpulse(state);
    }
}

/*
using Godot;

public class PlayerNewState : PlayerState
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
