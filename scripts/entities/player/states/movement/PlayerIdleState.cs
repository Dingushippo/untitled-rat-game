using Godot;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player owner)
        : base(owner) { }

    public override void Process(float delta)
    {
        Vector2 dir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        if (dir == Vector2.Zero)
            return;
        fsm.ChangeState<PlayerMoveState>();
    }

    public override void Enter(State previous = null)
    {
        _player.StickToFloor = true;
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump") && _player.IsOnFloor)
        {
            fsm.ChangeState<PlayerJumpState>(this);
        }
    }

    public override void Exit() { }
}
