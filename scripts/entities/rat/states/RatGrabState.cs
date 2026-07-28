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
        _rat.GlobalPosition = _player.ThrowComponent.GlobalPosition;
        _rat.Reparent(_player.ThrowComponent);
        _rat.Collider.Disabled = true;

        // Held rats sit right in front of the camera; leaving them on the interact layer
        // would swallow every raycast aimed at whatever the player is walking up to.
        _rat.InteractArea?.SetActive(false);
    }
    public override void Exit()
    {
        _rat.Reparent(_prevParent);
        _rat.Collider.Disabled = false;
        _rat.InteractArea?.SetActive(true);
    }
}