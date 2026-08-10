using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class RitualBase : Node3D, IPooledObject
{
    private const float DRAW_FREQ = 60f;
    [Export] public SubViewport Viewport;
    [Export] public RitualRenderer Renderer;
    [Export] public RitualResource RitualResource;
    [Export] public MeshInstance3D PlaneMesh;
    // [Export] public Array<IRitualTrigger> Triggers; // TODO implement


    public List<IRitualTrigger> Triggers = new();
    public Array<RitualElementSlot> Slots { get; set; }
    public Action<RitualBase> OnComplete;
    private readonly List<Tween> _animateTween = new();
    private FiniteStateMachine _fsm;
    public override void _Ready()
    {
        Renderer.Position = Viewport.Size / 2;
        _fsm = new(this);
        _fsm.Add("preview", new RitualPreviewState(this));
        _fsm.Add("idle", new RitualIdleState(this));
        _fsm.Add("active", new RitualActiveState(this));
        _fsm.Add("interrupted", new RitualInterruptedState(this));
        _fsm.Add("completed", new RitualCompletedState(this));
        _fsm.InitState("preview");
        _fsm.Debug = true;
    }

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);
    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);

    public void SetIdle()
    {
        _fsm.ChangeState("idle");
    }

    public void OnSpawn()
    {
        Show();
        SetProcess(true);
        SetPhysicsProcess(true);
    }

    public void OnDespawn()
    {
        Hide();
        RitualResource = null;
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    public void AnimateRats()
    {
        float rotation = Mathf.DegToRad(30);
        foreach (Rat rat in Slots.Select(x => x.WorkSlot.Occupant).ToList())
        {
            float startRotation = rat.Rotation.Y;
            Tween tween = CreateTween();
            tween.SetLoops();
            tween.TweenProperty(rat, "rotation:y", startRotation + rotation / 2, 0.2f);
            tween.TweenProperty(rat, "rotation:y", startRotation - rotation / 2, 0.2f);
            _animateTween.Add(tween);
        }
    }

    public void StopAnimation() => _animateTween.ForEach(x => x.Kill());
}
