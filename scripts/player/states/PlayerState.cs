public class PlayerState : State
{
    private protected Player _player;

    public PlayerState(Player owner) { _player = owner; }
}

/*
Template

public class NewPlayerState : PlayerState
{
    public NewPlayerState(Player owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null) { }
    public override void Exit() { }
}

*/