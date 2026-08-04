using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class QteManager : Control
{
    [Export] PackedScene[] QteScenes;

    private readonly Dictionary<string, PackedScene> _qtes = new();

    public override void _EnterTree()
    {
        foreach (PackedScene scene in QteScenes)
        {
            string fileName = scene.ResourcePath.Split("/")[^1].TrimSuffix(".tscn");
            _qtes[fileName] = scene;
        }
        EventBus.Subscribe(Event.StartQTE, OnQteStart);

    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.StartQTE, OnQteStart);
    }

    private void OnQteStart(object[] obj)
    {
        string @event = (string)obj[0];
        if (!_qtes.TryGetValue(@event, out PackedScene scene))
            GD.PushError($"Invalid QTE: {@event}");

        Action<bool> onCompleted = (Action<bool>)obj[1];
        QteBase qte = scene.Instantiate<QteBase>();
        qte.Completed = onCompleted;
        AddChild(qte);
    }
}