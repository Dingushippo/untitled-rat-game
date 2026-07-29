using Godot;


public class GameResultState : GameState
{
    private const string RESULT_SCENE_PATH = "res://scenes/UI/result.tscn";
    private Result _result;

    public GameResultState(GameManager owner) : base(owner) { }
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        if (previous is not GameRunState run)
        {
            GD.PushError("Something is wrong here, previous state is not run");
            return;
        }

        PackedScene resultScene = GD.Load<PackedScene>(RESULT_SCENE_PATH);
        _result = resultScene.Instantiate<Result>();
        _manager.GetTree().ChangeSceneToNode(_result);

        RunClock.Instance.Pause();
        Input.MouseMode = Input.MouseModeEnum.Visible;

        _result.RunSuccess.Text = run.RunSuccess ? "Winner winner" : "Epic fail";
        _result.RunStats.Text = GetStats(run);
        _result.Restart.ButtonUp += Restart;
        _result.MainMenu.ButtonUp += GoToMainMenu;
    }

    private string GetStats(GameRunState run)
    {
        int days = run.RunSuccess ? RunClock.Instance.Day : RunClock.Instance.Day - 1;
        return
            $"Days survived: {days}\n" +
            $"Stews delivered: {run.TotalStewsDelivered}\n" +
            $"Tithes collected: {EconomyService.Instance.Tithes}";
    }
    public override void Exit()
    {
        _result.Restart.ButtonUp -= Restart;
        _result.MainMenu.ButtonUp -= GoToMainMenu;
    }

    private void GoToMainMenu() => fsm.ChangeState("menu");
    private void Restart() => fsm.ChangeState("run");
}