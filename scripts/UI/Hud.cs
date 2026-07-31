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
    public override void _Ready()
    {
        EventBus.Subscribe(Event.ResourceChanged, OnResourceChanged);
        EventBus.Subscribe(Event.QuotaUpdated, OnQuotaUpdated);
        EventBus.Subscribe(Event.Sundown, OnSundown);
        SetDayLabel();
    }

    private void SetDayLabel() => DayLabel.Text = $"Day {RunClock.Instance.Day}";

    // Called every frame. 'delta' is the elapsed time since the previous frame.
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

    private void OnSundown(object[] args)
    {
        SetDayLabel();
    }


}
