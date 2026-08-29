using Godot;

public abstract partial class State : Node
{
    protected Node _owner;
    protected State _parent;
    protected HierarchicalStateMachine _hfsm;

    public virtual void Enter(State previous = null) { }

    public virtual void Exit() { }

    public virtual void PhysicsProcess(float delta) { }

    public virtual void Process(float delta) { }

    public virtual void Init(Node owner, HierarchicalStateMachine stateMachine, State parent = null)
    {
        _owner = owner;
        _hfsm = stateMachine;
        _parent = parent;
    }
}
