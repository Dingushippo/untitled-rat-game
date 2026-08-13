using Godot;
using Godot.Collections;
using System.Collections.Generic;


[GlobalClass]
public partial class SpamQte : QteBase
{
    // Called when the node enters the scene tree for the first time.
    [Export] bool Active = false;
    [Export] float ReductionSpeed = 100f;
    [Export] float ButtonPressIncrease = 10f;
    [Export] float StartTimer = 1f;
    [Export] Color EndColor = Colors.Green;
    [Export] Color StartColor = Colors.Red;

    [ExportGroup("Inputs used")]
    [Export(PropertyHint.InputName)] string InputAction;

    [ExportGroup("UI nodes")]
    [Export] public ProgressBar Progress;
    [Export] public Label LabelNode;
    [Export] public PanelContainer ActionPanel;

    public override void _Ready()
    {
        base._Ready();
        SetProgressBarColor();
        InputAction = InputAction != null ? InputAction : QteActions.GetRandomAction();
        LabelNode.Text = InputMap.ActionGetEvents(InputAction)[0].AsText().Replace("- Physical", "").StripEdges();
        GetTree().CreateTimer(StartTimer).Timeout += () => Active = true;
    }
    public override void _Process(double delta)
    {
        if (!Active) return;

        Progress.Value = Mathf.MoveToward(Progress.Value, 0, delta * ReductionSpeed);
        SetProgressBarColor();

        if (Progress.Value == Progress.MinValue) OnCompleted(false);
        else if (Progress.Value >= Progress.MaxValue) OnCompleted(true);
    }

    private void SetProgressBarColor()
    {
        float colorWeight = (float)(Progress.Value / Progress.MaxValue);
        Color currentColor = StartColor.Lerp(EndColor, colorWeight);
        StyleBoxFlat newStyleBox = new()
        {
            BgColor = currentColor
        };
        Progress.AddThemeStyleboxOverride("fill", newStyleBox);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && key.IsAction(InputAction))
        {
            if (!Active)
            {
                Active = true;
            }
            Tween tween = CreateTween();
            tween.SetTrans(Tween.TransitionType.Cubic);
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(Progress, "value", Progress.Value + ButtonPressIncrease, .1);
            tween.TweenProperty(ActionPanel, "offset_transform_scale", Vector2.One * 1.25f, .05);
            tween.TweenProperty(ActionPanel, "offset_transform_scale", Vector2.One, .3);
        }
    }


}
