using Godot;
using System;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;

public partial class Hud : Control
{

    [Export] public Label TitheLabel;
    [Export] public Label DayLabel;
    [Export] public Label QuotaLabel;
    [Export] public Label ClockLabel;
    [Export] public ProgressBar DayProgressBar;
    [Export] public ProgressBar FervorProgressBar;
    [Export] public PanelContainer InventoryContainer;
    [Export] public RichTextLabel InventoryLabel;

    public override void _EnterTree()
    {
        EventBus.Subscribe(Event.ResourceChanged, OnResourceChanged);
        EventBus.Subscribe(Event.RatPickedUp, OnRatPickedUp);
        EventBus.Subscribe(Event.RatReleased, OnRatReleased);
        EventBus.Subscribe(Event.QuotaUpdated, OnQuotaUpdated);
        EventBus.Subscribe(Event.DayStarted, OnDayStarted);
        EventBus.Subscribe(Event.ClockTick, OnClockTick);
    }
    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.ResourceChanged, OnResourceChanged);
        EventBus.Subscribe(Event.RatPickedUp, OnRatPickedUp);
        EventBus.Subscribe(Event.RatReleased, OnRatReleased);
        EventBus.Unsubscribe(Event.QuotaUpdated, OnQuotaUpdated);
        EventBus.Unsubscribe(Event.DayStarted, OnDayStarted);
        EventBus.Unsubscribe(Event.ClockTick, OnClockTick);
    }

    private Inventory _currentHeldInventory;
    private void UpdateInventory()
    {
        InventoryLabel.Text = IntventoryPrint.PrintContent(_currentHeldInventory);
    }
    private void OnRatReleased(object[] obj)
    {
        InventoryLabel.Text = "";
        _currentHeldInventory.Changed -= UpdateInventory;
        _currentHeldInventory = null;
        InventoryContainer.Hide();
    }

    private void OnRatPickedUp(object[] obj)
    {
        if (obj[0] is Rat rat)
        {
            _currentHeldInventory = rat.Cargo;
            InventoryContainer.Show();
            _currentHeldInventory.Changed += UpdateInventory;
            UpdateInventory();
        }
    }

    private void OnClockTick(object[] obj)
    {
        string timeText = (string)obj[0];
        float dayProgress = (float)obj[1];
        DayProgressBar.Value = dayProgress;
        ClockLabel.Text = timeText;
    }

    private void OnDayStarted(object[] obj)
    {
        int day = (int)obj[0];
        DayLabel.Text = $"Day {day}";
    }

    private void OnQuotaUpdated(object[] args)
    {
        string quotaText = $"Quota: {args[0]}/{args[1]}";
        QuotaLabel.Text = quotaText;
    }

    private void OnResourceChanged(object[] args)
    {
        if (args[0] is Economy type)
        {
            if (type == Economy.Tithes)
            {
                int newValue = (int)args[2];
                TitheLabel.Text = $"Tithes: {newValue}";
            }
            else if (type == Economy.Fervor)
            {
                int newValue = (int)args[2];
                FervorProgressBar.Value = newValue;
            }
        }
    }
}
