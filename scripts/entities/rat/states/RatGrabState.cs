using Godot;

public class RatGrabState : RatState
{
    private Node _prevParent;
    private Player _player;
    public RatGrabState(Rat owner) : base(owner) { }
    public void Configure(Player player)
    {
        _player = player;
    }
    public override void Enter(State previous = null)
    {
        _prevParent = _rat.GetParent();
        _rat.GlobalPosition = _player.ThrowComponent.HandNode.GlobalPosition;
        _rat.Reparent(_player.ThrowComponent.HandNode);
        _rat.Collider.Disabled = true;
        _rat.InteractArea?.SetActive(false);
        EventBus.Publish(new RatPickedUp(_rat));
    }
    public override void Exit()
    {
        _rat.Reparent(_prevParent);
        _rat.InteractArea?.SetActive(true);
        EventBus.Publish(new RatReleased(_rat));
    }


}