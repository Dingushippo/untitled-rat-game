using Godot;

public class RatGrabState : RatState
{
    private Node _prevParent;
    private Player _player;
    public RatGrabState(Rat owner, Player player) : base(owner) { _player = player; }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        _prevParent = _rat.GetParent();
        _rat.GlobalPosition = _player.ThrowAnchor.GlobalPosition;
        _rat.Reparent(_player.ThrowAnchor);
    }
    public override void Exit()
    {
        _rat.Reparent(_prevParent);
    }
}