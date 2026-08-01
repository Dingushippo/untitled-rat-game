using Godot;
using System;
using System.Diagnostics.Tracing;
using System.Runtime.Serialization;

public partial class Hud : Control
{

    [Export] public Label TitheLabel;
    [Export] public Label DayLabel;
    [Export] public Label QuotaLabel;
    [Export] public ProgressBar DayProgressBar;
    [Export] public ProgressBar FervorProgressBar;

    public override void _EnterTree()
    {
        EventBus.Subscribe(Event.ResourceChanged, OnResourceChanged);
        EventBus.Subscribe(Event.QuotaUpdated, OnQuotaUpdated);
        EventBus.Subscribe(Event.DayStarted, OnDayStarted);
        // EventBus.Subscribe(Event.Sundown, OnDayChanged);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.ResourceChanged, OnResourceChanged);
        EventBus.Unsubscribe(Event.QuotaUpdated, OnQuotaUpdated);
        EventBus.Unsubscribe(Event.DayStarted, OnDayStarted);
        // EventBus.Unsubscribe(Event.Sundown, OnDayChanged);
    }

    private void OnDayStarted(object[] obj)
    {
        int day = (int)obj[0];
        DayLabel.Text = $"Day {day}";
    }

    public override void _Process(double delta)
    {
        DayProgressBar.Value = RunClock.Instance.DayProgress;
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
