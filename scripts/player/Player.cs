using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [Export] public PlayerCamera camera;
    [Export] public Node3D aaa;


    private FiniteStateMachine _fsm;

    public override void _Ready()
    {
        InitStateMachine();
    }

    public override void _Process(double delta) => _fsm.StateProcess((float)delta);
    public override void _PhysicsProcess(double delta) => _fsm.StatePhysicsProcess((float)delta);
    public override void _UnhandledInput(InputEvent @event) => _fsm.StateUnhandledInput(@event);
    public override void _Input(InputEvent @event) => _fsm.StateInput(@event);


    private void InitStateMachine()
    {
        _fsm = new FiniteStateMachine();
        // _fsm.Add("follow", new RatFollowState(this, NavAgent));
        // _fsm.Add("target_reached", new RatTargetReachedState(this));
        // _fsm.Add("idle", new RatIdleState(this));
        _fsm.InitState("idle");
    }
}
