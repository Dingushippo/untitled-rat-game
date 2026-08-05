using Godot;
using System;

public partial class QteBase : Control
{
    public Action<bool> Completed;

    public override void _Ready()
    {
        EventBus.Publish(Event.QteStarted, this);
        GD.Print("Started QTE");
    }

    protected void OnCompleted(bool success)
    {
        Completed?.Invoke(success);
        EventBus.Publish(Event.QteCompleted, this);
        QueueFree();
    }
}

public static class QteActions
{
    public static string[] ValidActions = { "interact", "move_forward", "move_left", "move_right", "move_back", "jump" };

    public static string GetRandomAction()
    {
        Random rand = new();
        int index = rand.Next(ValidActions.Length);
        return ValidActions[index];
    }
}