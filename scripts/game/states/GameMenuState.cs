using Godot;
using System.Buffers;


public class GameMenuState : GameState
{
    private const string MAIN_MENU_PATH = "res://scenes/UI/main_menu.tscn";
    public GameMenuState(GameManager owner) : base(owner) { }
    public override void Enter(State previous = null)
    {
        RunClock.Instance.ResetTimer();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Callable.From(() => _manager.GetTree().ChangeSceneToFile(MAIN_MENU_PATH)).CallDeferred();
    }
    public override void HandleInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.E)
        {
            fsm.ChangeState("run");
        }
    }
}