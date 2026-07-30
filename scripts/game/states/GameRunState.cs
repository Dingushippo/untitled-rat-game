using Godot;
using System;


public class GameRunState : GameState
{
    private const string GAME_SCENE_PATH = "res://scenes/main.tscn";
    public GameRunState(GameManager owner) : base(owner) { }
    public bool RunSuccess = false;
    public int TotalStewsDelivered = 0;
    private int _stewsDeliveredToday = 0; // TODO change to more flexible quota system
    private int[] _quotas = { 5, 10, 20 };
    private int[] _ratsSpawnedPerDay = { 6, 2, 2 };
    private Node3D _level;
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        GD.Seed(1);

        EventBus.Subscribe(Event.ItemSold, OnItemSold);
        EventBus.Subscribe(Event.Sundown, OnSundown);
        EconomyService.Instance.ResetForRun();

        PackedScene levelScene = GD.Load(GAME_SCENE_PATH) as PackedScene;
        _level = levelScene.Instantiate<Node3D>();
        _level.Ready += OnLevelLoaded;

        _manager.GetTree().ChangeSceneToNode(_level);
    }

    private void OnLevelLoaded()
    {
        RunClock.Instance.Start();
        EventBus.Publish(Event.SpawnRat, _ratsSpawnedPerDay[RunClock.Instance.Day - 1]);
    }

    public override void Exit()
    {
        EventBus.Unsubscribe(Event.ItemSold, OnItemSold);
        EventBus.Unsubscribe(Event.Sundown, OnSundown);
        _level.Ready -= OnLevelLoaded;
        RunClock.Instance.ResetTimer();
    }
    private void OnItemSold(object[] obj)
    {
        string item = (string)obj[0];
        int amount = (int)obj[1];

        ItemDef itemDef = ItemDatabase.Get(item);
        int oldAmount = _stewsDeliveredToday;
        _stewsDeliveredToday += amount;
        TotalStewsDelivered += amount;

        EventBus.Publish(Event.ResourceChanged, itemDef, oldAmount, _stewsDeliveredToday);
    }
    private void OnSundown(object[] obj)
    {
        RunClock clock = RunClock.Instance;
        clock.IncrementDay();
        if (clock.Day <= _quotas.Length && _stewsDeliveredToday >= _quotas[clock.Day - 1])
        {
            ResetDay();
            EventBus.Publish(Event.DayStarted, clock.Day);
            EventBus.Publish(Event.SpawnRat, _ratsSpawnedPerDay[clock.Day - 1]);
        }
        else
        {
            fsm.ChangeState("result", this);
        }

    }

    private void ResetDay()
    {
        _stewsDeliveredToday = 0;
        RunClock.Instance.ResetTimer();
    }
}