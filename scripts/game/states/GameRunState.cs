using Godot;
using System;
using System.Reflection.Metadata.Ecma335;


public class GameRunState : GameState
{
    private const string GAME_SCENE_PATH = "res://scenes/main.tscn";
    public GameRunState(GameManager owner) : base(owner) { }
    public bool RunSuccess;
    public int TotalStewsDelivered = 0;
    public int StewsDeliveredToday
    {
        get => _stewsDeliveredToday;
        set
        {
            _stewsDeliveredToday = value;
            EventBus.Publish(Event.QuotaUpdated, _stewsDeliveredToday, GetCurrentQuota());
        }
    }
    private int _stewsDeliveredToday = 0;
    private Node3D _level;
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        ResetRunState();

        GD.Seed(_manager.Tuning.FixedSeed ? _manager.Tuning.Seed : (ulong)DateTime.Now.Ticks);

        EventBus.Subscribe(Event.ItemSold, OnItemSold);
        EventBus.Subscribe(Event.Sundown, OnSundown);
        EconomyService.Instance.ResetForRun();
        RunClock.Instance.ResetFull();

        PackedScene levelScene = GD.Load(GAME_SCENE_PATH) as PackedScene;
        _level = levelScene.Instantiate<Node3D>();
        _level.Ready += OnLevelLoaded;

        _manager.GetTree().ChangeSceneToNode(_level);
    }

    private void OnLevelLoaded()
    {
        RunClock.Instance.Start();
        EventBus.Publish(
            Event.SpawnRat,
            _manager.Tuning.RatsSpawnedPerDay[RunClock.Instance.Day - 1]
        );
        EventBus.Publish(Event.DayStarted, 1);
        EventBus.Publish(Event.QuotaUpdated, _stewsDeliveredToday, GetCurrentQuota());
    }

    public override void Exit()
    {
        EventBus.Unsubscribe(Event.ItemSold, OnItemSold);
        EventBus.Unsubscribe(Event.Sundown, OnSundown);
        _level.Ready -= OnLevelLoaded;
    }

    private int GetCurrentQuota() => _manager.Tuning.Quotas[RunClock.Instance.Day - 1];
    private void OnItemSold(object[] obj)
    {
        string item = (string)obj[0];
        int amount = (int)obj[1];

        ItemDef itemDef = ItemDatabase.Get(item);
        int oldAmount = StewsDeliveredToday;
        StewsDeliveredToday += amount;
        TotalStewsDelivered += amount;
    }
    private void OnSundown(object[] obj)
    {
        RunClock clock = RunClock.Instance;
        int quotaToMeet = GetCurrentQuota();
        GD.Print($"Stews delivered: {StewsDeliveredToday}/{quotaToMeet}, day {clock.Day}");
        if (StewsDeliveredToday < quotaToMeet)
        {
            GD.Print("Run success");
            fsm.ChangeState("result", this);
            return;
        }
        if (clock.Day == _manager.Tuning.Quotas.Length)
        {
            RunSuccess = true;
            GD.Print("Run success");
            fsm.ChangeState("result", this);
            return;
        }
        ResetDay();
        clock.Start();
    }

    private void ResetDay()
    {
        RunClock clock = RunClock.Instance;
        StewsDeliveredToday = 0;
        clock.IncrementDay();
        clock.ResetTimer();

        EventBus.Publish(Event.DayStarted, clock.Day);
        EventBus.Publish(Event.SpawnRat, _manager.Tuning.RatsSpawnedPerDay[clock.Day - 1]);
    }

    private void ResetRunState()
    {
        RunSuccess = false;
        TotalStewsDelivered = 0;
        _stewsDeliveredToday = 0;
        RunClock.Instance.ResetFull();
    }
}