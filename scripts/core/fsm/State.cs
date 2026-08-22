using Godot;

public abstract class State
{
    public virtual void PhysicsProcess(float delta) { }

    public virtual void Process(float delta) { }

    public virtual void HandleInput(InputEvent @event) { }

    public virtual void HandleUnhandledInput(InputEvent @event) { }

    public virtual void Enter(State previous = null) { }

    public virtual void Exit() { }
}

public abstract class State<T> : State
    where T : State<T>
{
    public FiniteStateMachine<T> fsm { get; internal set; }
}
