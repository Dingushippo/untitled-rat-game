using Godot;

public class State
{
    public FiniteStateMachine fsm;
    public virtual void PhysicsProcess(float delta) { }
    public virtual void Process(float delta) { }
    public virtual void HandleInput(InputEvent @event) { }
    public virtual void HandleUnhandledInput(InputEvent @event) { }
    public virtual void Enter(State previous = null) { }
    public virtual void Exit() { }
}