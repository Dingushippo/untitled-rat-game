public class PlayerState : State
{
    private protected Player _player;

    public PlayerState(Player owner) { _player = owner; }
}

/*
using Godot;

public class NewPlayerState : PlayerState
{
    public NewPlayerState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void HandleInput(InputEvent @event) { }
    public override void HandleUnhandledInput(InputEvent @event) { }
    public override void Exit() { }
}
*/