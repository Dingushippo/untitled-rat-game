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
        EventBus.Subscribe<StartQte>(OnQteStart);
        EventBus.Subscribe<QteCompleted>(OnQteCompleted);
    }

    public override void _ExitTree()
    {
        EventBus.Unsubscribe<StartQte>(OnQteStart);
    }

    private void OnQteStart(StartQte evt)
    {
        if (QteActive) return;

        if (!_qtes.TryGetValue(evt.Id, out PackedScene scene))
            GD.PushError($"Invalid QTE: {evt.Id}");

        QteBase qte = scene.Instantiate<QteBase>();
        qte.Completed = evt.OnComplete;
        AddChild(qte);

        _activeQte = qte;
    }

    private void OnQteCompleted(QteCompleted _)
    {
        _activeQte = null;
    }
}