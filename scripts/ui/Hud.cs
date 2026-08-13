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
        EventBus.Subscribe<ResourceChanged>(OnResourceChanged);
        EventBus.Subscribe(Event.RatPickedUp, OnRatPickedUp);
        EventBus.Subscribe(Event.RatReleased, OnRatReleased);
        EventBus.Subscribe<QuotaUpdated>(OnQuotaUpdated);
        EventBus.Subscribe<DayStarted>(OnDayStarted);
        EventBus.Subscribe<ClockTick>(OnClockTick);
    }
    public override void _ExitTree()
    {
        EventBus.Unsubscribe<ResourceChanged>(OnResourceChanged); ;
        EventBus.Unsubscribe(Event.RatPickedUp, OnRatPickedUp);
        EventBus.Unsubscribe(Event.RatReleased, OnRatReleased);
        EventBus.Unsubscribe<QuotaUpdated>(OnQuotaUpdated);
        EventBus.Unsubscribe<DayStarted>(OnDayStarted);
        EventBus.Unsubscribe<ClockTick>(OnClockTick);
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

    private void OnClockTick(ClockTick evt)
    {
        DayProgressBar.Value = evt.DayProgress;
        ClockLabel.Text = evt.Text;
    }

    private void OnDayStarted(DayStarted evt)
    {
        DayLabel.Text = $"Day {evt.Day}";
    }

    private void OnQuotaUpdated(QuotaUpdated evt)
    {
        string quotaText = $"Quota: {evt.Current}/{evt.Required}";
        QuotaLabel.Text = quotaText;
    }

    private void OnResourceChanged(ResourceChanged evt)
    {
        if (evt.Type == Economy.Tithes)
        {
            TitheLabel.Text = $"Tithes: {evt.NewVal}";
        }
        else if (evt.Type == Economy.Fervor)
        {
            FervorProgressBar.Value = evt.NewVal;
        }

    }
}
