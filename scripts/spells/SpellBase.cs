using Godot;
using System.Collections.Generic;


[GlobalClass]
public partial class SpellBase : Node3D
{
    [Export] private bool _debug;
    private SpellPayload _spellPayload;
    private Node3D _castNode;

    private ISpellComponent _currentComponent;
    private Queue<ISpellComponent> _componentQueue = new();

    public void Initialize(Node3D castNode, SpellPayload spellPayload)
    {
        _castNode = castNode;
        _spellPayload = spellPayload;

        GlobalPosition = castNode.GlobalPosition;

        foreach (Node child in GetChildren())
        {
            if (child is not ISpellComponent component)
                continue;

            if (_debug)
                GD.Print($"Adding component {component}");
            _componentQueue.Enqueue(component);
        }

        QueueNextComponent(_spellPayload);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentComponent?.Process((float)delta);
    }

    private void QueueNextComponent(SpellPayload payload)
    {
        if (_currentComponent != null)
            _currentComponent.OnComplete -= QueueNextComponent;
        if (_componentQueue.Count == 0)
        {
            _currentComponent = null;
            QueueFree();
            return;
        }
        _currentComponent = _componentQueue.Dequeue();
        _currentComponent.OnComplete += QueueNextComponent;

        if (_debug)
            GD.Print($"Queued into: {_currentComponent}, payload: {payload}, remaining in queue: {_componentQueue.Count}");
        _currentComponent.Initialize(this, payload);
    }
}