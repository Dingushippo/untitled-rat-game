public class HandState : State
{
    private protected Player _player;

    public HandState(Player owner) { _player = owner; }
}

/*
using Godot;

public class HandNewState : PlayerState
{
    public HandNewState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void HandleInput(InputEvent @event) { }
    public override void HandleUnhandledInput(InputEvent @event) { }
    public override void Exit() { }
}
*/