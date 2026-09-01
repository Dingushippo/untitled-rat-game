using Godot;
using System.Collections.Generic;


[GlobalClass]
public partial class SpellBase : Node3D
{
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
            GD.Print($"Spell completed: {Name}");
            _currentComponent = null;
            return;
        }
        GD.Print($"Queing from component: {_currentComponent}, payload: {payload}");
        _currentComponent = _componentQueue.Dequeue();
        _currentComponent.OnComplete += QueueNextComponent;
        _currentComponent.Initialize(this, payload);
        GD.Print($"Queued into: {_currentComponent.ComponentName}, payload: {payload}, remaining in queue: {_componentQueue.Count}");


    }
}