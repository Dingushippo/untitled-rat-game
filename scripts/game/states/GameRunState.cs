using Godot;
using Godot.Collections;
using System;
using System.Linq;


public class GameRunState : GameState
{
    private const string GAME_SCENE_PATH = "res://scenes/levels/test_level.tscn";
    public GameRunState(GameManager owner) : base(owner) { }
    public bool RunSuccess;
    public int TotalStewsDelivered = 0;
    public int StewsDeliveredToday
    {
        get => _stewsDeliveredToday;
        set
        {
            _stewsDeliveredToday = value;
            EventBus.Publish(new QuotaUpdated(_stewsDeliveredToday, GetCurrentQuota()));
        }
    }
    private int _stewsDeliveredToday = 0;
    private Node3D _level;
    private Dictionary<int, TimelineResource> _timelineDict;
    public override void Enter(State previous = null)
    {
        ResetRunState();

        _timelineDict = new Dictionary<int, TimelineResource>(_manager.Timeline.ToDictionary(x => x.Day));

        GD.Seed(_manager.Tuning.FixedSeed ? _manager.Tuning.Seed : (ulong)DateTime.Now.Ticks);

        EventBus.Subscribe<ItemSold>(OnItemSold);
        EventBus.Subscribe<Sundown>(OnSundown);
        EventBus.Subscribe(Event.ClockTick, HandleClockTick);
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
        EventBus.Publish(new SpawnRat(_manager.Tuning.RatsSpawnedPerDay[RunClock.Instance.Day - 1]));
        EventBus.Publish(new DayStarted(1));
        EventBus.Publish(new QuotaUpdated(_stewsDeliveredToday, GetCurrentQuota()));
    }

    public override void Exit()
    {
        EventBus.Unsubscribe<ItemSold>(OnItemSold);
        EventBus.Unsubscribe<Sundown>(OnSundown);
        EventBus.Unsubscribe(Event.ClockTick, HandleClockTick);
        _level.Ready -= OnLevelLoaded;
    }
    private int GetCurrentQuota() => _manager.Tuning.Quotas[RunClock.Instance.Day - 1];

    private void HandleClockTick(object[] args)
    {
        string tick = (string)args[0];
        int day = RunClock.Instance.Day;

        TimelineEvent[] currentEvents = _timelineDict[day].Events.Where(x => x.TimeStamp == tick).ToArray();

        if (currentEvents.Length == 0) return;

        foreach (TimelineEvent @event in currentEvents)
        {
            switch (@event.Type)
            {
                case TimelineEventType.Hazard:
                    string hazardId = (string)@event.Data["hazardId"];
                    EventBus.Publish(Event.SpawnHazard, hazardId);
                    break;
            }
        }
    }

    private void OnItemSold(ItemSold item)
    {
        ItemDef itemDef = ItemDatabase.Get(item.ItemId);
        int oldAmount = StewsDeliveredToday;
        StewsDeliveredToday += item.Amount;
        TotalStewsDelivered += item.Amount;
    }
    private void OnSundown(Sundown evt)
    {
        RunClock clock = RunClock.Instance;
        int quotaToMeet = GetCurrentQuota();
        GD.Print($"Stews delivered: {StewsDeliveredToday}/{quotaToMeet}, day {clock.Day}");
        if (StewsDeliveredToday < quotaToMeet)
        {
            fsm.ChangeState<GameResultState>(this);
            return;
        }
        if (clock.Day == _manager.Tuning.Quotas.Length)
        {
            RunSuccess = true;
            fsm.ChangeState<GameResultState>(this);
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

        EventBus.Publish(new DayStarted(clock.Day));
        EventBus.Publish(new SpawnRat(_manager.Tuning.RatsSpawnedPerDay[clock.Day - 1]));
    }

    private void ResetRunState()
    {
        RunSuccess = false;
        TotalStewsDelivered = 0;
        _stewsDeliveredToday = 0;
        RunClock.Instance.ResetFull();
    }
}