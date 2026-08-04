using Godot;
using System;

public partial class QteBase : Control
{
    public Action<bool> Completed;

    public override void _Ready()
    {
        EventBus.Publish(Event.QTEStarted, this);
        GD.Print("Started QTE");
    }

    protected void OnCompleted(bool success)
    {
        Completed?.Invoke(success);
        EventBus.Publish(Event.QTECompleted, this);
        QueueFree();
    }
}