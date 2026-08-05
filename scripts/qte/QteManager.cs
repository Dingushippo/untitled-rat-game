using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class QteManager : Control
{
    [Export] PackedScene[] QteScenes;

    private readonly Dictionary<string, PackedScene> _qtes = new();

    private QteBase _activeQte = null;

    public bool QteActive => _activeQte != null;

    public override void _EnterTree()
    {
        foreach (PackedScene scene in QteScenes)
        {
            string fileName = scene.ResourcePath.Split("/")[^1].TrimSuffix(".tscn");
            _qtes[fileName] = scene;
        }
        EventBus.Subscribe(Event.StartQte, OnQteStart);
        EventBus.Subscribe(Event.QteCompleted, OnQteCompleted);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe(Event.StartQte, OnQteStart);
    }

    private void OnQteStart(object[] obj)
    {
        if (QteActive) return;

        string @event = (string)obj[0];
        if (!_qtes.TryGetValue(@event, out PackedScene scene))
            GD.PushError($"Invalid QTE: {@event}");

        Action<bool> onCompleted = (Action<bool>)obj[1];
        QteBase qte = scene.Instantiate<QteBase>();
        qte.Completed = onCompleted;
        AddChild(qte);

        _activeQte = qte;
    }

    private void OnQteCompleted(object[] obj)
    {
        _activeQte = null;
    }
}