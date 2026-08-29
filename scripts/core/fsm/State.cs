using Godot;

public abstract partial class State : Node
{
    protected Node _owner;

    public virtual void Enter(State previous = null) { }

    public virtual void Exit() { }

    public virtual void PhysicsProcess(float delta) { }

    public virtual void Process(float delta) { }

    public virtual void HandleInput(InputEvent @event) { }
}

public abstract partial class TypedState<T> : State
    where T : TypedState<T>
{
    protected HierarchicalStateMachine<T> _hfsm;

    public virtual void Init(Node owner, HierarchicalStateMachine<T> stateMachine)
    {
        _owner = owner;
        _hfsm = stateMachine;
    }
}
