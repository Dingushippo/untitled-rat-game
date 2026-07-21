using Godot;

public class State
{
    public FiniteStateMachine fsm;
    public virtual void PhysicsProcess(float delta) { }
    public virtual void Process(float delta) { }
    public virtual void Input(InputEvent @event) { }
    public virtual void UnhandledInput(InputEvent @event) { }
    public virtual void Enter(State previous = null) { }
    public virtual void Exit() { }
}