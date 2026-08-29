using Godot;

public partial class GameMenuState : GameState
{
    private const string MAIN_MENU_PATH = "res://scenes/ui/main_menu.tscn";

    public override void Enter(State previous = null)
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Callable.From(() => _manager.GetTree().ChangeSceneToFile(MAIN_MENU_PATH)).CallDeferred();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.E)
        {
            if (GameManager.Instance.HasFatalDataError)
                OS.Alert("Fatal data error, check output");
            else
                _hfsm.ChangeState<GameRunState>();
        }
    }

    // public override void HandleInput(InputEvent @event)
    // {
    //     if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.E)
    //     {
    //         if (GameManager.Instance.HasFatalDataError)
    //             OS.Alert("Fatal data error, check output");
    //         else
    //             _hfsm.ChangeState<GameRunState>(this);
    //     }
    // }
}
