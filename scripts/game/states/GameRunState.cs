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
    public override void PhysicsProcess(float delta) { }
    public override void Process(float delta) { }
    public override void Enter(State previous = null)
    {
        EventBus.Subscribe(Event.ItemSold, OnItemSold);
        EventBus.Subscribe(Event.Sundown, OnSundown);
        EconomyService.Instance.ResetForRun();

        _manager.GetTree().ChangeSceneToFile(GAME_SCENE_PATH);
        RunClock.Instance.Start();
    }
    public override void Exit()
    {
        EventBus.Unsubscribe(Event.ItemSold, OnItemSold);
        EventBus.Unsubscribe(Event.Sundown, OnSundown);
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
        if (_stewsDeliveredToday >= _quotas[clock.Day - 1] && clock.Day <= _quotas.Length)
        {
            ResetDay();
            clock.IncrementDay();
            EventBus.Publish(Event.DayStarted, clock.Day);
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