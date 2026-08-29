using Godot;

public abstract partial class State : Node
{
    protected Node _owner;
    protected State _parent;

    public virtual void Enter(State previous = null) { }

    public virtual void Exit() { }

    public virtual void PhysicsProcess(float delta) { }

    public virtual void Process(float delta) { }
}

public abstract partial class TypedState<T> : State
    where T : TypedState<T>
{
    protected HierarchicalStateMachine<T> _hfsm;

    public virtual void Init(Node owner, HierarchicalStateMachine<T> stateMachine, State parent = null)
    {
        _owner = owner;
        _parent = parent;
        _hfsm = stateMachine;
    }
}
