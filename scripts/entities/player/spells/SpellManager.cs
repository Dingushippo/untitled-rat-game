using Godot;
using System.Collections.Generic;

public partial class SpellManager : Node
{
    [Export] public SpellResource CurrentSpellResource;
    [Export] public SpellResource[] SpellResources;

    private List<Spell> _activeSpells;
    private Spell _currentSpell;
    private Node _currentScene;

    private float _chargeTimer;
    private bool _isCharging;
    public override void _Ready()
    {
        _currentScene = GetTree().CurrentScene;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isCharging)
        {
            _chargeTimer += (float)delta;
            _currentSpell?.ProcessCharging();
        }

        if (_activeSpells.Count == 0)
            return;

        foreach (Spell spell in _activeSpells)
            spell.ProcessActive();
    }

    public void StartSpellCharge()
    {
        _currentSpell = CurrentSpellResource.SpellScene.Instantiate<Spell>();
        _currentScene.AddChild(_currentSpell);
        _currentSpell.OnSpawn();

        _chargeTimer = 0f;
        _isCharging = true;
    }

    public void Cast() // on release
    {
        _isCharging = false;
        _activeSpells.Add(_currentSpell);
        _currentSpell.OnRelease();
        _currentSpell = null;
    }
}