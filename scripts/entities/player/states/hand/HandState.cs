public class HandState : State<HandState>
{
    private protected Hand _hand;

    public HandState(Hand owner)
    {
        _hand = owner;
    }
}

/*
using Godot;

public class HandNewState : State<HandState>
{
    public HandNewState(Hand owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void HandleInput(InputEvent @event) { }
    public override void HandleUnhandledInput(InputEvent @event) { }
    public override void Exit() { }
}
*/
